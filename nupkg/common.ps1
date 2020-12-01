# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$rootFolder = Join-Path $packFolder "../"

$solutions = (
	"src/"
)

$projects = (
	# framework
	"src/Wechaty",
	"src/external-modules/Wechaty.EventEmitter",
	"src/external-modules/Wechaty.FileBox",
	"src/external-modules/Wechaty.MemoryCard",
	"src/external-modules/Wechaty.ReactiveQueue",
	"src/external-modules/Wechaty.Watchdog",
	"src/external-modules/Wechaty.Common",
	"src/external-modules/Wechaty.Puppet",
	"src/external-modules/Wechaty.Puppet.Hostie"
	

)