# Change log

### 2020-12
- Upgrade .NET5
- Adjust to extract shared items
- Adjust Netnr.Login to remove Netnr.Core dependency
- Adjust Netnr.WeChat to remove Netnr.Core dependency

### 2020-11
- Adjust the method of resetting the database of the Netnr.ResponseFramework project (and delete the memory database)
- New Netnr.Fast class library ActionResultVM class adds log collection object, used to fill log output, add fragment time-consuming method PartTime

### 2020-10
- Repair Netnr.Logging class library UV statistics (the first IP statistics)
- Added Netnr.Blog.Web project log to add search
- Fix the thread insecure problem of the Count attribute of the log cache queue

### 2020-09
- Fixed Netnr.ResponseFramework.Web project table configuration drag and drop sorting reset can not be used
- Adjust the Gist code snippet of Netnr.Blog.Web project to respond to scroll wheel events
- Update Netnr.Guff attachment hosting
- Upgrade Netnr.FileServer project to support time-limited token and permanent token authorization
- Adjusted SVG icon merge of Netnr.ScriptService project
- Adjust the log cache of Netnr.ResponseFramework.Web project to Queue
- Adjusted Netnr.Logging project log cache to Queue
- Integrated database access as a class library Netnr.Data

### 2020-08
- Added a new `ToView()` method to the OSInfoTo.cs class library of the Netnr.Fast.Extend project to visualize output
- New test item Netnr.Test
- Integrate Netnr.Login.Sample project into Netnr.Test
- Integrate Netnr.WeChat.Sample project into Netnr.Test
- Delete the laboratory module of the Netnr.Blog.Web project