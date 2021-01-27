# Netnr.DataKit (NDK)
Database build code

> <https://dk.netnr.eu.org>  
> <https://netnr-dk.azurewebsites.net>


### Functional
- Supported databases: MySQL, SQLite, Oracle, SQLServer, PostgreSQL
- Load and export table information and column information
- Modify table and column comments
- Query and export table data
- Build code based on language templates, support csharp, java, php, etc.
- The language template construction is based on the `JS` script and supports debugging scripts before building
- Support extended language template, extended language template object: `dk.build.language`, type mapping: `dk.build.typeMapping`

### Progress of language template construction
- [x] csharp/model (generate C# corresponding entity)
- [x] csharp/dal (generate C# corresponding data access method, add, delete, modify, etc.)
- [ ] java (java series)
- [ ] php (php series)

> Since I am a `.NET` developer, I am not very familiar with `java` and `php.` If you are interested, you can build a familiar language template and integrate it.