
# Define the base path
$basePath = "c:\Users\human\Documents\DQ_like\Assets\Scripts\Date\"
$enemiesPath = Join-Path $basePath "Enemies"
$databasePath = Join-Path $basePath "Field_01_Enemies\EnemyDatabase_01.asset"

# Ensure Enemies directory exists
if (-not (Test-Path $enemiesPath)) {
    New-Item -ItemType Directory -Path $enemiesPath | Out-Null
}

# Define enemy data
$enemies = @(
    @{ Name = "Enemy_Bat"; ID = 1; DisplayName = "\u30B3\u30A5\u30E2\u30EA"; MaxHP = 15; AttackMin = 2; AttackMax = 5; Description = "A small bat."; GUID = "e824cbf24121494d8cbfbad0f6295d11" },
    @{ Name = "Enemy_Spider"; ID = 2; DisplayName = "\u30B9\u30D1\u30A4\u30C0\u30FC"; MaxHP = 25; AttackMin = 4; AttackMax = 8; Description = "A poisonous spider."; GUID = "6f7d175358624d11ad27adaa69fe6a08" },
    @{ Name = "Enemy_Skeleton"; ID = 3; DisplayName = "\u30B9\u30B1\u30EB\u30C8\u30F3"; MaxHP = 40; AttackMin = 8; AttackMax = 12; Description = "A rattling skeleton."; GUID = "be9a890cfd1c4bfb9e56cefb46927836" },
    @{ Name = "Enemy_Ghost"; ID = 4; DisplayName = "\u30B4\u30FC\u30B9\u30C8"; MaxHP = 35; AttackMin = 6; AttackMax = 10; Description = "A spooky ghost."; GUID = "5c044987fdea48518183e6dc2d969f7e" },
    @{ Name = "Enemy_Orc"; ID = 5; DisplayName = "\u30AA\u30FC\u30AF"; MaxHP = 60; AttackMin = 10; AttackMax = 15; Description = "A brutish orc."; GUID = "de55d7ad4aa747588e87df3a402a401a" },
    @{ Name = "Enemy_Mage"; ID = 6; DisplayName = "\u30E1\u30A4\u30B8"; MaxHP = 30; AttackMin = 12; AttackMax = 18; Description = "A dark mage."; GUID = "4020bbe90c964d0596dc912a41abfdf5" },
    @{ Name = "Enemy_Wolf"; ID = 7; DisplayName = "\u30A6\u30EB\u30D5"; MaxHP = 45; AttackMin = 9; AttackMax = 13; Description = "A wild wolf."; GUID = "72a4bc1d6216407597124e0171805d50" },
    @{ Name = "Enemy_Golem"; ID = 8; DisplayName = "\u30B4\u30FC\u30EC\u30E0"; MaxHP = 100; AttackMin = 15; AttackMax = 20; Description = "A stone golem."; GUID = "ffc160ec6da3444881b1c6d19bf30a99" },
    @{ Name = "Enemy_Dragon"; ID = 9; DisplayName = "\u30C9\u30E9\u30B4\u30F3"; MaxHP = 200; AttackMin = 25; AttackMax = 35; Description = "A fierce dragon."; GUID = "88a5299bdd0c42d28468f304070c25bf" },
    @{ Name = "Enemy_Knight"; ID = 10; DisplayName = "\u30CA\u30A4\u30C8"; MaxHP = 80; AttackMin = 18; AttackMax = 22; Description = "A fallen knight."; GUID = "f0a95b765bc549bdbbc773ff9307d731" }
)

# Template for .asset file (Escaped curly braces)
$assetTemplate = @"
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: 8571b5d5686c36449b53884cdfba97fb, type: 3}}
  m_Name: {0}
  m_EditorClassIdentifier: 
  EnemyID: {1}
  DisplayName: "{2}"
  MaxHP: {3}
  AttackMIn: {4}
  AttackMax: {5}
  Description: {6}
"@

# Template for .asset.meta file (Escaped curly braces)
$metaTemplate = @"
fileFormatVersion: 2
guid: {0}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"@

# Create asset and meta files
foreach ($enemy in $enemies) {
    $assetPath = Join-Path $enemiesPath ($enemy.Name + ".asset")
    $metaPath = $assetPath + ".meta"

    try {
        $assetContent = $assetTemplate -f $enemy.Name, $enemy.ID, $enemy.DisplayName, $enemy.MaxHP, $enemy.AttackMin, $enemy.AttackMax, $enemy.Description
        $metaContent = $metaTemplate -f $enemy.GUID

        Set-Content -Path $assetPath -Value $assetContent -Encoding UTF8
        Set-Content -Path $metaPath -Value $metaContent -Encoding UTF8
        
        Write-Host "Created $($enemy.Name)"
    } catch {
        Write-Error "Failed to create $($enemy.Name): $_"
    }
}

# Update EnemyDatabase
Write-Host "Updating EnemyDatabase at $databasePath"

$databaseContent = Get-Content -Path $databasePath -Raw

foreach ($enemy in $enemies) {
    if ($databaseContent -match $enemy.GUID) {
        Write-Host "Enemy $($enemy.Name) already in database."
    } else {
        $entry = "  - {fileID: 11400000, guid: " + $enemy.GUID + ", type: 2}`r`n"
        $databaseContent += $entry
        Write-Host "Added $($enemy.Name) to database."
    }
}

Set-Content -Path $databasePath -Value $databaseContent -Encoding UTF8
Write-Host "Database update complete."
