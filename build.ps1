
Properties {
	$buildDir = Split-Path $psake.build_script_file	
	$buildArtifactsDir = "$buildDir\_build"
	$projectOutputDir = "$buildArtifactsDir\bin\"
	$solutionDir = "$buildDir"
	$version = "0.5.1"
}

Task default -Depends Compile, RunTests, CreatePackage

Task Compile -Depends Clean {
	Write-Host "Building solution" -ForegroundColor Green
	Exec { msbuild "$solutionDir\CsvParser\CsvParser.fsproj" /t:Build /p:Configuration=Release /v:quiet /p:OutDir=$projectOutputDir } 
}

Task Clean {
	Write-Host "Creating BuildArtifacts directory" -ForegroundColor Green
	if (Test-Path $buildArtifactsDir) 
	{	
		rd $buildArtifactsDir -rec -force | out-null
	}
 	  
	mkdir $buildArtifactsDir | out-null
	
	Write-Host "Cleaning solution" -ForegroundColor Green
	Exec { msbuild "$solutionDir\CsvParser.sln" /t:Clean /p:Configuration=Release /v:quiet } 
}

Task RunTests {
	"TODO: Run tests"	
}

Task CreatePackage {
	Write-Host "Building NuGet package" -ForegroundColor Green
	&"$solutionDir\.nuget\nuget.exe" pack "$solutionDir\CsvParser.nuspec" -OutputDirectory "$buildArtifactsDir" -Version $version
}
