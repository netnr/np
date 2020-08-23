# Change log

### [v1.4.0] - 2020-08-23
- Adjust the `CacheTo` class to depend on the `System.Runtime.Caching` component
- Adjust the framework version to `NET45`, `.NETStandard2.0`

### [v1.3.0] - 2020-05-16
- Adjusted the `FileTo` class read/write method to pass parameters, the original file path and file name were transferred separately, and now the complete physical path is transferred together
- Removed some methods of executing Shell on `CmdTo` class, leaving only basic methods

### [v1.2.3] - 2020-01-06
- Adjust to support `.NET Standard 2.0`, to support `.NET Framework 4.6` + references

### [v1.2.2] - 2019-12-22
- Adjusted the `ConsoleTo` class to add type judgments, fixed newline characters being serialized as strings