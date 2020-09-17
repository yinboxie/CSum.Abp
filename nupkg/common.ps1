# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$rootFolder = Join-Path $packFolder "../"

# List of solutions
$solutions = (
    ""
)

# List of projects
$projects = (

    # framework
    "src/CSum.Abp.AspNetCore.Mvc",
    "src/CSum.Abp.Data",
    "src/CSum.Abp.Ddd.Application",
    "src/CSum.Abp.Ddd.Application.Contracts",
    "src/CSum.Abp.Ddd.Domain",
    "src/CSum.Abp.EntityFrameworkCore",
    "src/CSum.Abp.EntityFrameworkCore.SqlServer",
    "src/CSum.Abp.SwaggerUI",
    "src/CSum.Abp.Util"
)
