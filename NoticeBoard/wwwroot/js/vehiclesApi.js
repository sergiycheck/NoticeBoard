//ts-check
'use strict'

//!!!always dont forget to change ngrok link
const uri = 'https://localhost:5003/api/todoitems';
//'https://34aa978a0475.ngrok.io/api/todoitems'
//'https://localhost:5001/api/todoitems';

let todos =[];

// function getHeaders()
// {
//     var headers = new Headers();
//     headers.append('Content-Type', 'application/json');
//     headers.append('Accept', 'application/json');
//     headers.append('Access-Control-Allow-Origin', uri);
//     headers.append('Access-Control-Allow-Credentials', 'true');
//     headers.append('GET', 'POST', 'OPTIONS');
//     return headers;
// }


function getItems(){
    fetch(uri,{
        method:'GET',
        // credentials:'include',
        // headers:getHeaders()
    })
    .then(response=>response.json())
    .then(data=>_displayItems(data))
    .then(()=>displayMessageOnFetchedElements())
    .catch(error=>console.log('Unable to get items.',error));
}

function _displayItems(data){
    const tableBody = document.getElementById('todos');
    tableBody.innerHTML='';

    _displayCount(data.length);

    const button = document.createElement('button');
    

    data.forEach(element => {
        let isCompleteCheckBox = document.createElement('input');
        isCompleteCheckBox.type='checkbox';
        isCompleteCheckBox.disabled=true;
        isCompleteCheckBox.checked = element.isComplete;

        let editButton = button.cloneNode(false);
        editButton.textContent='Edit';
        editButton.setAttribute('onclick',`displayEditForm(${element.id})`);
        editButton.className = 'btn btn-warning';
        

        let deleteButton = button.cloneNode(false);
        deleteButton.textContent = 'Delete';
        deleteButton.setAttribute('onclick',`deleteItem(${element.id})`);
        deleteButton.className = 'btn btn-danger';
        

        let tableRow = tableBody.insertRow(-1);

        let td1 = tableRow.insertCell(0);
        td1.appendChild(isCompleteCheckBox);

        let td2 = tableRow.insertCell(1);
        let textNode = document.createTextNode(element.name);
        td2.appendChild(textNode);

        let td3 = tableRow.insertCell(2);
        td3.appendChild(editButton);    

        let td4 = tableRow.insertCell(3);
        td4.appendChild(deleteButton);
        
    });
    todos = data;
}
function _displayCount(itemsCount){
    const name =(itemsCount===1)?'to-do':'to-dos';
    document.getElementById('counter').innerHTML = `${itemsCount} ${name}`;
}

function displayEditForm(id){
    const item = todos.find(item=>item.id===id);

    document.getElementById('edit-name').value=item.name;
    document.getElementById('edit-id').value=item.id;
    document.getElementById('edit-isComplete').value=item.isComplete;
    document.getElementById('editForm').style.display = 'block';

}

function deleteItem(id){
    fetch(`${uri}/${id}`,{
        method: 'DELETE'
    })
    .then(()=>getItems())
    .catch(error=>console.error(`Unable to delete item with id ${id}`,error));
}

function addItem(){
    const addNameTextBox = document.getElementById('add-name');
    
    const item = {
        isComplete: false,
        name: addNameTextBox.value.trim()
    };

    fetch(uri,{
        method:'POST',
        headers:{
            'Accept':'application/json',
            'Content-Type':'application/json'
        },
        body:JSON.stringify(item)
    })
    .then(response=>response.json())
    .then(()=>{
        getItems();
        addNameTextBox.value='';
    })
    .catch(error=>console.error(`Unable to add item ${item}`,error));
}

function updateItem(){
    const itemId = document.getElementById('edit-id').value;

    const item = {
        id : parseInt(itemId,10),
        isComplete : document.getElementById('edit-isComplete').checked,
        name : document.getElementById('edit-name').value.trim()
    };

    fetch(`${uri}/${itemId}`,{
        method:'PUT',
        headers:{
            'Accept':'application/json',
            'Content-Type':'application/json'           
        },
        body:JSON.stringify(item),
    })
    .then(()=>getItems())
    .catch(error=>console.error(`Unable to add item ${item}`,error));

    closeInput();

    return false;
}

function closeInput(){
    document.getElementById('editForm').style.display = 'none';
}


function displayMessageOnFetchedElements(){

    let container = document.getElementById("counter");
    let span = document.createElement('span');
    span.innerText=' fetched elements';
    container.appendChild(span);
}