# Change log

### [The change log has been migrated](CHANGELOG.md)

### [v3.1.5] - 2020-06-14
- Optimization configuration menu link does not need to be lowercase (but it is recommended to uniform lowercase)
- Added the background check whether the role can access the link that has been added to the menu table

### [v3.1.4] - 2020-05-18
- Adjust the project structure
- Fix page deletion method `POST` request changed to `GET`
- Adjust the name of the `init.js` method; the link of the tag tab A is the address of the `iframe`, which is convenient for right-clicking and opening a new window
- Adjust the log table to add `User-Agent`, and introduce the `DeviceDetector.NET` component to better analyze the **operating system** and **browser** information of the client
- Adjust table management to provide services using the `Netnr.DataKit` project

### [v3.1.3] - 2020-04-26
- Adjust the project structure

### [v3.1.2] - 2020-04-18
- Adjust component CDN source
- Adjust and upgrade the reference package

### [v3.1.1] - 2019-12-22
- Adjust database context to connection pool hosting
- Adjust IP address query to `IP2Region` component

### [v3.1.0] - 2019-12-04
- Adjust and upgrade to `.NET Core 3.1`
- Add server information page

### [v3.0.1] - 2019-11-13
- Adjust all non-page requests to be standardized into interfaces, and use swagger to generate visual interface documents, <https://rf2.netnr.com/swagger>

### [v3.0.0] - 2019-09-29
- Adjust and upgrade to `.NET Core 3.0`

### [v2.2.10] - 2019-07-23
- Add `QueryWhere` method overload for `IEnumerable` conditional query (support some query relation symbols)
- Adjusted table management to support search (query based on `IEnumerable`, ie memory search)
- Adjust the date and time components to support **placeholder** display
- Fix the logic defect of batch switching method `z.batchButtonSwitch`
- Add documents (display in master-slave table, such as purchase order, more component support must be improved)
- Adjustment Adjustment of parameter transfer of form components is compatible with documents, that is, it supports the generation of modal forms and page switching documents
- Adjust the internal method structure of `z.js` to solve some problems
- Adjustment Introduce timing task component **FluentScheduler**
- Add Automatically generate single table addition, deletion, modification and query code (Controller, View, JavaScript)

### [v2.2.9] - 2019-07-05
- Fix that the service of the scheduled task is not added to the container, resulting in the scheduled task not being executed

### [v2.2.8] - 2019-06-25
- Add `Data Dictionary`
- Add an example of embedding rich text in a form (based on `CKEditor4`, pictures support paste, drag and drop upload, size limit **2MB**)
- Adjust the binding of z.Combo to support **placeholder** display

### [v2.2.7] - 2019-06-23
- Fix public export (js script problem)
- Add upload example (ajax upload, with real upload progress bar, form display format, view file, required verification)
- Add a batch operation example (depending on Z.EntityFramework.Extensions)

### [v2.2.6] - 2019-06-14
- Adjust input and output entity object names to provide support for public export
- Adjust the public export query dependent query, only maintain one place
- Adjustments, adding, deleting and modifying unified return format, maintenance method return object
- Adjust the `appsettings.json` configuration file to set the key name `TypeDB` to be equal to the database type
- Added `PostgreSQL` scripts to generate table configuration and table information query

### [v2.2.5] - 2019-06-10
- Fix the path problem of export and file cleaning on linux
- Fix the logic problem that the built-in parameter can ignore the log record
- Fixed an issue where a null value was assigned when the multi-select drop-down list value was empty when editing the form
- Added a new item for table configuration: form `maxlength` attribute
- Added table configuration new item: `query condition relation symbol` configuration, to avoid unsupported relation symbol query error
- Add function button of Iframe page `Full Screen`
- Added low version browser blocking prompt page
- Added drop-down list panel highly adaptive
- Adjust the DataGrid paging component not to display the flip page, and enable the shortcut page number
- Adjust the page form configuration, the drag sorting method is changed from node exchange to node insertion
- Adjustment: When the window size changes, change from modifying the height of all iframe tags to only modifying the height of the current iframe tags, and then trigger when switching
- Adjustment For better adaptive effect, set `scrolling="no"` in the iframe tag, and use `<div class="bodyscroll">` packaging for scroll bar display on subpages, refer to the desktop page

### [v2.2.4] - 2019-06-02
- Added role to add copy role permissions function
- Add `Public Export`, customize query, condition, format
- Add timed tasks, automatically reset the database, clean up temporary directories
- Add a table example, the quick query and the query panel coexist; note: the conditions for enabling the quick query need to be closed in the query panel to avoid conflicts
- Add table example, `batch delete` function example
- Adjusted the optimization of the `batch processing` function button, and the batch state function button is automatically switched
- Adjust the default relationship of the query panel from **equal** to **include**

### [v2.2.3] - 2019-04-26
- Adjust tab page to add control menu
- Adjust the rounded corners to `1px`
- Adjust `EasyUI` to refer to `metro` theme by default
- Adjustment The introduction of static resources is uniformly maintained by the partial view
- Add the log table to add the city to which the IP belongs, and to query the IP address offline, based on the free `ipip.ipdb`
- **According to the voting results initiated by the group, only the `.NET Core` version will be updated in the future, and `.NET4` will not be updated**

### [v2.2.2] - 2019-04-18
- Adjust the log write in batches

### [v2.2.1] - 2019-02-28
- Adjustment Export the `NPOITo` class of Excel, support `.xls`, `.xlsx` export
- Adjusted table query panel generation mode, based on table column configuration, supports multi-table query panel
- Remove the shortcut key `Ctrl + Q` to open the query panel (because there are multiple query panels)
- Add table column headings to quickly open the positioning query panel
- Adjustment Run the project for the first time, automatically create a database, adjust and reset the database (the `scripts/table-reset` directory needs a corresponding database script)
- Fix the problem of assignment of datetime date and time input type query panel (it did not prevent bubbling, first finish editing and then assign value causing assignment failure)
- Adjust the `Netnr.Core` class library project imported from NuGet, and add the `Netnr.Fast` project
- Adjustments to support multi-form generation (the form and form configuration of the page are the first by default, and multi-form and form configuration are not currently supported)

### [v2.2.0] - 2019-01-17
- Adjust the optional list of Grid rows, 200 rows are optional
- Fix the problem that multiple Grid parameters are interfering. The repair method is to clear the `queryparams` object first
- Adjustment Removed the date and time plug-in `bootstrap-datetimepicker` and changed it to `EasyUI` to provide date and time components, reducing two requests
- Add Modal pop-up window to set the `autorefresh` property, and automatically refresh every time the `iframe` is opened
- Adjusted Moda1 pop-up window to default to full-screen pop-up window
- Adjust the prompt of mandatory fields in the form to red star
- Add Chinese language pack
- Fix the problem of different styles when the page nesting is greater than two levels. The fix is ​​to change the `parent` object to `top`
- Adjust the `init.css` style package to embed `z.css`
- Adjust `easel-custom.css` The adjusted style is embedded in `z.css`, and the page directly imports the original package `easyul.css`
- Adjust table configuration hiding to add system level hiding
- Adjust Grid line number width adaptive
- Fix the problem of abnormal switching user menu, menu cache problem, remove menu cache
- Added table management export `Excel`
- Adjust the naming of database table fields to reduce some problems with duplicate columns in related queries
- Adjustment The demo project database was migrated from `MySQL` to `SQLServer`, no other reason, mainly because of your own convenience
- Adjustment The default relation symbol in the query panel is "equal to", and the shortcut key `Ctrl + Q` is supported to open the query panel and `Enter` to confirm the search

### [v2.1.2] - 2018-11-09
- Add `.NET Framework 4.5` framework source code, update Core version synchronously

### [v2.1.2] - 2018-11-01
- Fix the problem of row number when the `DataGrid` executes `updateRow` to update the page (the reason why pageNumber is not set)
- Adjust some operations of public query

### [v2.1.2] - 2018-10-27
- Added `Button Management` function
- Added `Menu Management` function
- Add `Authorization Association` function (support third-party login)
- Add `z.Combo` method to add clear value button support
- Add `PostgreSQL` database support
- Adjust the `Linq` query, the sorting sequence may not be transmitted, that is, the default sorting
- The right side of the adjustment tab is just a refresh button

### [v2.1.1] - 2018-10-13
- Fix the problem of method `z.FindTreeNode`
- Fix the problem of method `z.FormEdit`
- Fix the problem of `Linq` query first pagination and then sorting
- Fix the problem that the system operation log, the `IP` acquisition is always `127.0.0.1`, the reason is the `nginx` proxy, the proxy environment needs to be judged

### [v2.1.0]　2018-10
- Add cross-platform and cross-database support, <https://github.com/aspnet/EntityFrameworkCore>, tested `SQLite`, `MySql`, `SQLServer`
- Generating entities depends on the `Scaffold-DbContext` command <https://www.netnr.com/gist/code/5283651389582691979>
- Adjust the public query from SQL statement to Linq
- Fix some issues with `z.js`
- Add table management tool to generate table configuration and table dictionary
- Adjustment Adjustment of the demo project, from `SQLServer` to `MySql`, server migration to foreign masonry (Centos7, Nginx, MySql, CN2 lines)
- 
- By the way, I have been using a `Windows` server. After contacting `Linux`, I think that `Linux` is a good server, and I plan to migrate all to the `Linux` server.
  - `Linux` server is cheap, relatively speaking
  - Most of the foreign `VPS` are limited to flow and speed (such as 1G bandwidth, 1T monthly flow), of course, the line is also important, otherwise the delay and packet loss will be serious
  - No need to file, you can take a ladder over the wall
  - `Linux` boot occupies about `100`MB, and running a `dotnet` process is only about `300`MB
  - `.net framework` projects can also run, `mono`, [jexus](https://www.jexus.org/)
  - `SQLServer` database already has `Linux` version, of course it can be changed to `MySql`
  - Getting started with the `Linux` server, with a dumb face, I don’t know where to start, what to do, what do you say, salad, there are a lot of tutorials on the Internet, understand one and count one; here are some I use s things:
    - ssh command to connect to the server
    - Installation environment: `dotnet`, `nginx`, `ftp`, `mysql`, `frp` (WeChat development), `shadowsocks` (over the wall)
    - Learn `vi` editor, learn `dotnet` command, learn `nginx` configuration
    - Some dry goods in stock <https://www.netnr.com/gist/user/1>

### [v2.0.0] - 2018-07
- The front end adopts jQuery + Bootstrap + EasyUI + AceAdmin
- Backend adopts Asp.Net Core + EF + SQLServer
- Brand new rewrite of `z.js` script package, consistent with the API provided by EasyUI (maximum adjustment)
- Rewrite iframe tab
- Streamlined ace navigation