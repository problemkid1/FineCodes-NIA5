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
    const btnAddMembership = document.getElementById("btnAddMembership");
    const btnRemoveMembership = document.getElementById("btnRemoveMembership");

    if (btnAddMembership && document.getElementById("availableMembership") && document.getElementById("selectedMembership")) {
        btnAddMembership.addEventListener("click", (event) =>
            switchOptions(event, document.getElementById("availableMembership"), document.getElementById("selectedMembership")));
    }

    if (btnRemoveMembership && document.getElementById("selectedMembership") && document.getElementById("availableMembership")) {
        btnRemoveMembership.addEventListener("click", (event) =>
            switchOptions(event, document.getElementById("selectedMembership"), document.getElementById("availableMembership")));
    }

    // Industry Buttons
    const btnAddIndustry = document.getElementById("btnAddIndustry");
    const btnRemoveIndustry = document.getElementById("btnRemoveIndustry");

    if (btnAddIndustry && document.getElementById("availableIndustry") && document.getElementById("selectedIndustry")) {
        btnAddIndustry.addEventListener("click", (event) =>
            switchOptions(event, document.getElementById("availableIndustry"), document.getElementById("selectedIndustry")));
    }

    if (btnRemoveIndustry && document.getElementById("selectedIndustry") && document.getElementById("availableIndustry")) {
        btnRemoveIndustry.addEventListener("click", (event) =>
            switchOptions(event, document.getElementById("selectedIndustry"), document.getElementById("availableIndustry")));
    }

    // Contact Buttons
    const btnAddContact = document.getElementById("btnAddContact");
    const btnRemoveContact = document.getElementById("btnRemoveContact");

    if (btnAddContact && document.getElementById("availableContact") && document.getElementById("selectedContact")) {
        btnAddContact.addEventListener("click", (event) =>
            switchOptions(event, document.getElementById("availableContact"), document.getElementById("selectedContact")));
    }

    if (btnRemoveContact && document.getElementById("selectedContact") && document.getElementById("availableContact")) {
        btnRemoveContact.addEventListener("click", (event) =>
            switchOptions(event, document.getElementById("selectedContact"), document.getElementById("availableContact")));
    }

    // Form Submission
    const form = document.querySelector("form");
    if (form) {
        form.addEventListener("submit", function () {
            // Select all options in the relevant select boxes before submitting
            ["selectedMembership", "selectedIndustry", "selectedContact"].forEach(id => {
                const selectElement = document.getElementById(id);
                if (selectElement) {
                    Array.from(selectElement.options).forEach(opt => opt.selected = true);
                }
            });
        });
    }
});
