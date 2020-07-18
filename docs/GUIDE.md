# Development Guide (2020-05-16)
Better project management, better start-up projects, and better handover projects

![Project Layered Architecture](static/project/la.png)

### Site presentation layer structure
- `wwwroot` directory
    - Directory of public static resources
- `wwwroot/css` directory
    - Style root directory of all pages
    - All lower case
    - `global.css` global style
    - Page style storage format: `{controller}/{action}.css`, such as: `/home/index.css`
- `wwwroot/images` directory
    - Picture root directory of all pages
- `wwwroot/js` directory
    - Script root directory of all pages
    - All lower case
    - `global.js` global script
    - Page script storage similar to css structure
    - Page style storage format: `{controller}/{action}.js`, such as: `/home/index.js`
- `wwwroot/lib` directory
    - Common component root directory
    - Such as the referenced jquery package, jquery/xxx.js
    - Different versions of the same component are in a directory, such as jquery/jquery-1.0.js, jquery/jquery-2.0.js
    - Components try to extract dist files, such as examples, maps, themes, language packs, etc. are not introduced as much as possible (only necessary items are kept)
- `wwwroot/upload` directory
    - Upload file root directory
    - Uploaded files need to be classified, that is, files cannot be stored directly under the `upload` directory
    - Commonly cited:
    - `temp` directory, where all temporary files are stored, such as imported files and exported files, you can empty the `temp` directory at any time
    - `template` directory, templates, such as print templates, import templates
    - `docs` directory, uploaded documents, such as upload interface can achieve similar storage: `/docs/2019/05/id.docx`, depending on the specific situation
    - Remember to store all files in the same directory
    - To facilitate applications such as load balancing, when using files, configure the server address + file path, such as `static.domain.com` + `/upload/docs/2019/05/id.docx`
- `Components` directory
    - Components
- `Controllers` directory
    - Controller, corresponding to the first menu of the system
    - When building a project, create a controller based on the first-level menu
    - When new functional modules appear, discuss whether a new controller is needed
    - The controller is named after the big hump, such as `AccountController`; the abbreviation can be all capitalized, such as API
    - Translate the corresponding English name according to the Chinese name of the function module
    - Controller class must add notes
- `Filters` directory
    - filter
    - Such as authorized access, diary records, etc.
- `logs` directory
    - Journal
    - Error log, debug log, the path is as follows: `2019/05/20190515.log`, depending on the situation, but `2019/05` is the molecular path by year and month is necessary
    - Avoid excessively large single log files and excessive number of files in a directory
- `Views` directory
    - View
    - The folder corresponding to the controller, which contains all the `Action` pages
    - `Action` keeps the big hump name, like `Index`
    - Each `Action` must have a comment description
    - If there is a public partial view under the controller, create `_PartialXXXX.cshtml`, starting with `_Partial`
- `Views/Shared` directory
    - Public view
    - Global partial view starting with `_Partial`
- `appsettings.json` file
    - Configuration file
    - Each item must be annotated
    - By category
    - Example:
    
```
{
    //Connection string
    "ConnectionStrings": {
        "MySQL": "Server=localhost;uid=root;pwd=123;database=db;",
        "SQLServer": "Server=.;uid=sa;pwd=123;database=db;",
        "PostgreSQL": "Host=localhost;Username=postgres;Password=123;Database=db;"
    },

    //Log
    "logs": {
        //Open the log
        "enable": true,
        //path
        "path": "~/logs",
        //Write in batches to satisfy any trigger
        "CacheWriteCount": 9999,
        //Write time in batches (seconds) to satisfy any trigger
        "CacheWriteSecond": 9
    }
}
```

### Database and projects
- Database design, recommend `PD`, or bring your own design tools
- The table name is named after the big hump, without symbols (underscore, horizontal bar, etc.)
- Keep the prefixes consistent with the classification table names, such as: System tables: `SysUser`, `SysRole`, `SysLog`
- Table names are translated according to Chinese, pinyin cannot be used
- Table fields are translated according to Chinese, Pinyin cannot be used
- Tables and table fields must be annotated, the primary key can be ignored, the key field should be described clearly, such as: `Gender (1: male; 2: female)`
- The primary key uniformly uses the `GUID` string, and the database type is a string; exceptions for special requirements
- Primary and foreign key associations are not recommended; exceptions for special requirements
- Write view is not recommended; exceptions for special requirements
- Writing stored procedures is not recommended; exceptions for special requirements
- Table index creation, evaluation setting field is clustered index, non-clustered index
- VS recommends and implements rewriting of project code, with exceptions in certain circumstances
- vs. unnecessary prompts for the project, delete
- Vs prompts for variables not needed for the project, delete
- All `Action`s need to be commented; all class methods must be commented, and key places are commented;
- One method handles multiple complex logics, which need to be described in order
- Generally no exception handling is required, the global filter will record exception logs; in some cases, you need to handle exceptions yourself, and you should try to output exception logs
- `JS` scripts, all methods must have key comments; reduce the declaration of global variables, reduce the declaration of variables in the same scope, too much will lead to conflicts and annoyances
- All methods must have only one line interval, Action, library methods, JS functions, etc.
- An empty line, generally means to open the block, the code is beautiful, like a comma segment
- Two empty lines, generally means hierarchical, similar to a semicolon in a paragraph
- Keep consistent with the overall code style of the project, do not highlight your own code style
- Method parameters, small hump first
- The project construction and naming recommends the form of `Aaa.Bbb`, such as `Volo.Abp`
- Code decoupling, function subdivision, can improve the utilization rate, convenient and better maintenance and upgrade
- Priority to use `LINQ`, complex functions, some queries that do not support `LINQ`, can use `SQL` query string processing (prevent injection)
- Cross-platform, enhanced portability: can easily switch databases, can easily switch deployment environment