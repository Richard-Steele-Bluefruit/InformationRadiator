# Information Radiator

&copy; Bluefruit 2015

Welcome to the Bluefruit Information Radiator

---

## Configuration

**This section is currently incomplete**

An example configuration file is shown below.

    <?xml version="1.0"?>
    <InformationRadiatorConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        <Items>
            <InformationRadiatorItem ItemType="TeamCityMetrics" Width="1030" Height="400" Left="0" Top="0">
                <Title>Code Metrics</Title>
                <Configuration>
                    <Field ID="URL">http://127.0.0.1:8080/data.csv</Field>
                <Configuration>
            </InformationRadiatorItem>
        </Items>
    </InformationRadiatorConfiguration>


---

## Contributing guidelines

### Development practices

* All code must be written in C# using Visual Studio 2013
* This project uses Continuous Integration, you must merge your code into the
  main line branch (master) at least once a day. There is a CI server setup,
  see Byran to get access.
* All code must meet the following metrics
  * Total code coverage : 85%
  * Max depth : 6
  * Max cyclomatic complexity : 20
  * Maximum run time of tests : 20 seconds
* All commits must compile and have all the tests passing. This means that no
  work in progress commits are allowed to end up in the master branch. If
  you do create a work in progress commit then squash it before merging it to
  master.
* Do not add source code for libraries to this repository, use NuGet (or a
  binary package if no NuGet package is available).

### Version Numbering
The Information Radiator uses Semantic Versioning 2.0.0
([link](http://semver.org/)), to fit in with Windows version numbering only the
first three digits of the assembly version number are used and the last digit
is always set to 0. (i.e. the version number format is
&lt;major&gt;.&lt;minor&gt;.&lt;revision&gt;.0)

### Adding new projects

* The unit test assembly names must end in ".Tests", this ensures that the
  build script will automatically pick up new unit test assemblies on the CI
  server.
* There is a global version number, add (as a link) the VersionInfo.cs file in
  the root of the repository to the project and remove the duplicated fields
  from the projects AssemblyInfo.cs file.
