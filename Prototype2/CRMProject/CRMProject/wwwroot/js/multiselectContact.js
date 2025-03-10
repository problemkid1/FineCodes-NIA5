document.addEventListener('DOMContentLoaded', function () {
    let DDLforChosen = document.getElementById("selectedOptions");
    let DDLforAvail = document.getElementById("availOptions");

    /*function to switch list items from one ddl to another
    use the sender param for the DDL from which the user is multi-selecting
    use the receiver param for the DDL that gets the options*/
    function switchOptions(event, senderDDL, receiverDDL) {
        let senderID = senderDDL.id;
        let selectedOptions = document.querySelectorAll(`#${senderID} option:checked`);
        event.preventDefault();

        if (selectedOptions.length === 0) {
            alert("Nothing to move.");
        }
        else {
            selectedOptions.forEach(function (o, idx) {
                senderDDL.remove(o.index);
                receiverDDL.appendChild(o);
            });
        }
    }

    // Create closures so that we can access the event & the 2 parameters
    let addOptions = (event) => switchOptions(event, DDLforAvail, DDLforChosen);
    let removeOptions = (event) => switchOptions(event, DDLforChosen, DDLforAvail);

    // Assign the closures as the event handlers for each button
    let btnLeft = document.getElementById("btnLeft");
    let btnRight = document.getElementById("btnRight");
    let btnSubmit = document.getElementById("btnSubmit");

    // Ensure the buttons exist before adding event listeners
    if (btnLeft) btnLeft.addEventListener("click", addOptions);
    if (btnRight) btnRight.addEventListener("click", removeOptions);
    if (btnSubmit) btnSubmit.addEventListener("click", function () {
        DDLforChosen.childNodes.forEach(opt => opt.selected = "selected");
    });
});
