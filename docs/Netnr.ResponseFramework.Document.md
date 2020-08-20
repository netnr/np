### z.Grid()
The methods and attributes are basically the same as the official documents

```js
var gd1 = new z.Grid();

gd1.isinit  //It is true for the first time, and it is always false after binding
gd1.id  //Table container#ID, default #Grid1
gd1.type    //Table type, default datagrid, support: datagrid, treegrid, propertygrid, datalist
gd1.autosize    //Automatically adjust the size, default xy, support: xy (wide, full, high and sinking), x, y, p (filling the parent container)
gd1.autosizePid //Parent container #ID, default #myBody, required for automatic resizing
gd1.columnsExists   //Whether to query the column configuration, set the value to true after the first query column configuration, if it is true, the query column configuration will not be followed
gd1.dataCache   //Data obtained by asynchronous request

gd1.onComplete(function(obj){ })    //Complete the callback, which can be called multiple times. Note: Calling the binding again within the completion will cause an infinite loop, which needs to be marked out

gd1.bind()  //Local binding method
gd1.load()  //Load method, bind method will be called automatically after loading

gd1.func()  //Method call, consistent with the official method, example:
gd1.func('getSelected') //Get the selected row
gd1.func('updateRow', (index:1, row:()))    //Update row

//Query panel component expansion
gd1.queryMark   //Create query mark, default true
gd1.queryData   //The query condition item, the title column is extracted by default, and can be customized (array type); you can also set the (virtual) table name (string type), that is, if you enter the query condition and then query the data, the configuration table name will Synchronize request form configuration to get query condition items
gd1.QueryBuild()    //Create a query panel (including initializing the query panel and assigning gd1.query, it will only be created when the gd1.query object is empty)
gd1.QueryOpen(field)    //Open the query panel (automatically call the gd1.QueryBuild method); field, optional, point to the query condition field name, locate the row and enable editing
gd1.QueryWhere()    //Get the conditions of the query panel
gd1.QueryOk = function () {}    //Click the callback of the query panel confirmation, optional, customize the confirmation event, if you donâ€™t write this event, the first page of the query panel condition request will be obtained by default
gd1.query   //The query object is the query panel object; it will have a value after calling gd1.QueryBuild() or gd1.QueryOpen()
z.queryin   //Point to the open query panel, when calling gd1.QueryOpen() method, z.queryin = gd1.query, open other query panels will point to the corresponding
gd1.query.grid  //Query panel table, the same type of object as gd1
gd1.query.id    //Point to the container ID of the query panel table
gd1.query.md    //Query panel modal box object, the object returned by new z.Modal()
```

Query panel component expansion

### z.GridQueryMark(gd)

> Create query mark

### z.GridQueryBuild(gd)

> Create query panel

### z.GridQueryOpen(gd, field)

> Open the query panel, automatically call the GridQueryBuild creation method, parameter: field specifies a query column field, optional

### z.GridQueryWhere(gd)

> Get the query conditions of the query panel to form the z.SqlQuery object



### z.GridFormat()

> Common formatting methods, such as gender, date, status, amount, etc.

### z.GridAuto(gd)

> Grid resize method

### z.GridLoading(gd)

> When the table is loaded for the first time, the loading icon is displayed, and subsequent loading prompts are provided by the DataGrid component

### z.GridEditor(gd, index, field, row)

> Grid edit configuration, parameter row is optional, see script comments for details

### z.GridEditorBlank(gd)

> Click blank to end Grid editing


### z.Combo()

> The methods and attributes are basically the same as the official documents, and have the same form as z.Grid

```js
var cb1 = new z.Combo();

cb1.type    //Type, default combobox, support: combobox, combotree tree

cb1.onComplete(function(obj){ })    //Complete the callback, which can be called multiple times. Note: Calling the binding again within the completion will cause an infinite loop, which needs to be marked out

cb1.bind()  //Local binding method
cb1.load()  //Load method, bind method will be called automatically after loading

cb1.func()  //Method call, consistent with the official method, example:
cb1.func('getValue')    //Get value
cb1.func('setValue', 1) //assignment
```

### z.TreeVagueCheck(cb, values)

> Tree is selected fuzzy, the child nodes used for a node are partially selected, after assignment, the child nodes are all selected, use this method to deal with, the example refers to the role permission setting

### z.FormAttrAjax()

> formui form request return data source & callback, used to initialize the data source bound by asynchronous request, used with z.FormAttrBind method

### z.FormAttrBind(target)

> formui form type source binding, used to initialize the corresponding component method according to different types and use with z.FormAttrAjax method

### z.FormRequired(color, FormId, dialog)

> The form is required for verification, used when saving

### z.FindTreeNode(data, value, key)

> Find tree nodes

### z.FormEdit(rowData)

> Backfill form, used to select a row of the form, directly assign the form to edit

### z.FormToJson(FormId)

> The form is converted to JSON, which is used for editing and saving, without refreshing and loading, and directly obtaining the form value to update a row of data

### z.FormClear(FormId)

> Clear the form for adding

### z.FormDisabled(dd, FormId)

> Disable forms for viewing

### z.FormAutoHeight()

> Adjust the height of the modal frame sheet for the height adaptation of the modal frame

### z.FormTitle(ops)

> Form title setting ops example: {id:"#id", title:"new", required:true}
* icon title icon
* title title
* id title container ID or object
* required Whether to display the required prompt text, the default is true

### z.Modal()

> Create a modal box

```js
var md1 = new z.Modal();

md1.okText  //ok button text
md1.cancelText  //cancel button text
md1.okClick //Ok click callback
        
md1.cancelClick //Cancel click callback
md1.title   //title content
md1.content //Main content
md1.src //iframe address, overwrite content attribute
md1.heightIframe    //iframe height
md1.complete    //iframe loading complete callback
md1.size    //Modal size Default 2 Optional: 1|2|3|4; Corresponding respectively (sm, md, lg, full)
md1.showClose   //Display the close button in the upper right corner
md1.showFooter  //Show footer
md1.showCancel  //Display the Cancel button

md1.okClick = function(){}  //Determine the event
md1.cancelClick = function(){}  //cancel

md1.append()    //Append to the body, the method is called after the final attribute assignment
md1.show()  //show
md1.hide()  //Hide
md1.remove()    //Remove

md1.modal   //Point to the jQuery object of the modal box, such as: md1.modal.find('div.modal-body') Find the content body of the modal box
```
### z.SqlQuery()

> Objects used to represent SQL query conditions

```
// id1='1' AND id2 IN('1','2') AND id2 LIKE'%5%' AND id3>='5' AND id3<='10'
[
    {
        field: "id1",
        relation: "Equal",
        value: 1
    },
    {
        field: "id2",
        relation: "In",
        value: [1, 2]
    },
    {
        field: "id2",
        relation: "Contains",
        value: "5"
    },
    {
        field: "id3",
        relation: "BetweenAnd",
        value: [5, 10]
    }
]

// description of relation symbol
{
    Equal:'{0} = {1}',
    NotEqual:'{0} != {1}',
    LessThan:'{0} <{1}',
    GreaterThan:'{0}> {1}',
    LessThanOrEqual:'{0} <= {1}',
    GreaterThanOrEqual:'{0} >= {1}',
    BetweenAnd:'{0} >= {1} AND {0} <= {2}',
    Contains:'%{0}%',
    StartsWith:'{0}%',
    EndsWith:'%{0}',
    In:'IN',
    NotIn:'NOT IN'
}
```

### z.DC

> Page data cache, including components, data sources, etc., everything is inside
* The drop-down list, according to the lowercase of the requested URL address as the key, stores the information of the drop-down list component

### z.btnTrigger

> Button trigger identification, click the corresponding function button, assign the corresponding button ID

### z.button(type, fn)

> Click the button event (if you need to disable some function buttons, add the disabled style to the button to take effect, instead of setting the button disable attribute), not applicable to the secondary button (pop-up drop-down menu button)
```js
z.button('add',functin(){ console.log('New event') })
```

### z.buttonClick(type)

> Simulate operation button click, such as: z.buttonClick('add') Simulate click to add

### art(content, fnOk, fnCacle)

> Message prompt, inquiry prompt, depend on z.Modal
```js
art('Save successfully'); //Similar to the alert method
//There are several keywords that have been converted: fail, error, success, select
art('fail') //The operation failed, generally used to return the result is failure
art('error') //Network error, generally used for asynchronous request error event
art('success') //The operation is successful, generally used to save successfully
//Maintain a unified style for all prompts and avoid some: wrong operation, operation failure, system error, etc.

art('Are you sure to delete?', function(){
    //OK, initiate a delete request
})
art('Does it cover?', function(){
    //cover
},function(){
    //Do not cover
})
```