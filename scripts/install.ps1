[CmdletBinding()]
param(
    [string]$MsixPath,
    [string]$CertPath,
    [switch]$Force,
    [switch]$Machine
)

$ErrorActionPreference = 'Stop'

function Resolve-ArtifactPath {
    param(
        [string]$Path,
        [string[]]$Patterns,
        [string]$Label
    )

    if ($Path) {
        if (-not (Test-Path -LiteralPath $Path)) {
            throw "$Label not found: $Path"
        }
        return (Resolve-Path -LiteralPath $Path).Path
    }

    $searchRoots = @($PWD.Path, $PSScriptRoot) | Select-Object -Unique
    foreach ($root in $searchRoots) {
        foreach ($pattern in $Patterns) {
            $item = Get-ChildItem -Path $root -File -Filter $pattern -ErrorAction SilentlyContinue |
                Sort-Object LastWriteTime -Descending |
                Select-Object -First 1
            if ($item) {
                return $item.FullName
            }
        }
    }

    $locations = $searchRoots -join ', '
    throw "$Label not found. Place it under: $locations."
}

function Ensure-CertificateInStore {
    param(
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        [string]$StoreName,
        [string]$StoreLocation
    )

    $storePath = "Cert:\$StoreLocation\$StoreName"
    try {
        $existing = Get-ChildItem $storePath | Where-Object { $_.Thumbprint -eq $Certificate.Thumbprint }
        if (-not $existing) {
            Write-Host "Importing certificate into ${StoreLocation}\${StoreName}: $($Certificate.Subject)"
            $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($StoreName, $StoreLocation)
            $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
            $store.Add($Certificate)
            $store.Close()
        } else {
            Write-Host "Certificate already installed in ${StoreLocation}\${StoreName}."
        }
    } catch {
        Write-Host "Failed to access ${StoreLocation}\${StoreName}. Try running as administrator. $($_.Exception.Message)"
        throw
    }
}

$msixPath = Resolve-ArtifactPath -Path $MsixPath -Patterns @('*.msixbundle', '*.msix') -Label 'MSIX'
$signature = Get-AuthenticodeSignature -FilePath $msixPath
$signerCertificate = $signature.SignerCertificate
if ($signerCertificate) {
    Write-Host "Package signer: $($signerCertificate.Subject) ($($signerCertificate.Thumbprint))"
    if ($signature.Status -ne 'Valid') {
        Write-Host "Signature status: $($signature.Status) ($($signature.StatusMessage))"
    }
}

$certPath = $null
$certFromFile = $null
if ($CertPath) {
    $certPath = Resolve-ArtifactPath -Path $CertPath -Patterns @('PhotoGeoExplorer.cer', '*.cer') -Label 'Certificate (CER)'
    $certFromFile = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certPath)
} elseif (-not $signerCertificate) {
    $certPath = Resolve-ArtifactPath -Path $null -Patterns @('PhotoGeoExplorer.cer', '*.cer') -Label 'Certificate (CER)'
    $certFromFile = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certPath)
}

$certificateToTrust = $signerCertificate
if (-not $certificateToTrust) {
    $certificateToTrust = $certFromFile
} elseif ($certFromFile -and $certFromFile.Thumbprint -ne $signerCertificate.Thumbprint) {
    Write-Host "Warning: certificate mismatch with package signature. Using signer certificate."
}

if (-not $certificateToTrust) {
    throw "Certificate not found. Provide -CertPath or place a .cer next to the MSIX."
}

Ensure-CertificateInStore -Certificate $certificateToTrust -StoreName 'TrustedPeople' -StoreLocation 'CurrentUser'
Ensure-CertificateInStore -Certificate $certificateToTrust -StoreName 'Root' -StoreLocation 'CurrentUser'
Ensure-CertificateInStore -Certificate $certificateToTrust -StoreName 'TrustedPublisher' -StoreLocation 'CurrentUser'

if ($Machine) {
    Ensure-CertificateInStore -Certificate $certificateToTrust -StoreName 'TrustedPeople' -StoreLocation 'LocalMachine'
    Ensure-CertificateInStore -Certificate $certificateToTrust -StoreName 'Root' -StoreLocation 'LocalMachine'
    Ensure-CertificateInStore -Certificate $certificateToTrust -StoreName 'TrustedPublisher' -StoreLocation 'LocalMachine'
}

if ($Force) {
    Get-AppxPackage -Name 'PhotoGeoExplorer' | ForEach-Object {
        Write-Host "Removing existing package: $($_.PackageFullName)"
        Remove-AppxPackage -Package $_.PackageFullName
    }
}

Write-Host "Installing MSIX: $msixPath"
Add-AppxPackage -Path $msixPath
Write-Host "Done."
