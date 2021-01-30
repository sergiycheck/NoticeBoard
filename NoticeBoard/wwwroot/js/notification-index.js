"use strict"

//Jquery table
//https://codepen.io/nathancockerill/pen/OQyXWb
//ASP.NET CORE MVC AJAX FORM REQUESTS USING JQUERY-UNOBTRUSIVE
//https://damienbod.com/2018/11/09/asp-net-core-mvc-ajax-form-requests-using-jquery-unobtrusive/

clickListener("notf-name");
clickListener("notf-descipt");

function clickListener(idName) {
    document.getElementById(idName).addEventListener('click', () => {
        //handle sort
        sortTable(idName);
    });
}

function sortTable(idName) {
    let table = document.getElementById("sorting-table");
    let rows = table.rows;
    //get number of row with a element with id idName
    console.log(rows);
    let head = rows[0];
    let links = head.getElementsByTagName("a");
    let ids = [];
    let dir = "asc";
    for (const iterator of links) {
        if (iterator.id) {
            ids.push(iterator.id);
        }
    }
    let colNum = ids.indexOf(idName);
    console.log(ids, "\n", `index of ${idName} = `, colNum);
    let switching = true;
    let shouldSwitch = false;
    let i;
    let wasSwitches = false;
    while (switching) {
        switching = false;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;//if no switches dont switch last row
            let cellFirst = rows[i].getElementsByTagName("td")[colNum];
            let cellSecond = rows[i + 1].getElementsByTagName("td")[colNum];

            let dataFirst = cellFirst.innerHTML.toLowerCase().trim();
            let dataSecond = cellSecond.innerHTML.toLowerCase().trim();
            if (dir === "asc") {
                if (dataFirst > dataSecond) {
                    shouldSwitch = true;
                    break;
                }
            } else if (dir === "desc") {
                if (dataFirst < dataSecond) {
                    shouldSwitch = true;
                    break;
                }
            }
        }

        if (shouldSwitch) {
            //switch rows
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            wasSwitches = true;
        } else if (!wasSwitches && dir === "asc") {//on ordered table switches again
            dir = "desc";
            switching = true;
        }
    }
}

const searchInput = document.getElementById("search-table-inpt");
searchInput.addEventListener('keyup', (event) => {
    event.preventDefault();
    let searchValue = searchInput.value.toLowerCase();
    let table = document.getElementById("sorting-table");
    let tbody = table.getElementsByTagName("tbody");
    let bodyRows = tbody[0].rows;
    for (let i = 0; i < bodyRows.length; i++) {
        let htmlRowCollection = bodyRows[i].getElementsByTagName("td");
        //console.log(htmlRowCollection);
        let wasNotFound = false;
        for (let j = 0; j < htmlRowCollection.length; j++) {
            let cellData = htmlRowCollection[j].innerHTML.trim().toLowerCase();
            if (cellData.indexOf(searchValue) > -1) {
                wasNotFound = false;
                bodyRows[i].style.display = "";
                break;
            } else {
                wasNotFound = true;
            }
        }
        if (wasNotFound) {
            bodyRows[i].style.display = "none";
        }

    }

}, false)