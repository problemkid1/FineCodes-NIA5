document.addEventListener("DOMContentLoaded", function () {
    function switchOptions(event, senderDDL, receiverDDL) {
        event.preventDefault();
        let selectedOptions = [...senderDDL.options].filter(opt => opt.selected);

        if (selectedOptions.length === 0) {
            alert("Nothing to move.");
            return;
        }

        selectedOptions.forEach(option => {
            senderDDL.remove(option.index);
            receiverDDL.appendChild(option);
        });
    }

    // Membership Buttons
    document.getElementById("btnAddMembership").addEventListener("click", (event) =>
        switchOptions(event, document.getElementById("availableMembership"), document.getElementById("selectedMembership")));

    document.getElementById("btnRemoveMembership").addEventListener("click", (event) =>
        switchOptions(event, document.getElementById("selectedMembership"), document.getElementById("availableMembership")));

    // Industry Buttons
    document.getElementById("btnAddIndustry").addEventListener("click", (event) =>
        switchOptions(event, document.getElementById("availableIndustry"), document.getElementById("selectedIndustry")));

    document.getElementById("btnRemoveIndustry").addEventListener("click", (event) =>
        switchOptions(event, document.getElementById("selectedIndustry"), document.getElementById("availableIndustry")));

    // Form Submission
    document.getElementById("btnSubmit").addEventListener("click", function () {
        ["selectedMembership", "selectedIndustry"].forEach(id => {
            Array.from(document.getElementById(id).options).forEach(opt => opt.selected = true);
        });
    });
});