$(document).ready(function () {
    // Contact selection functionality
    $("#btnAddContact").click(function () {
        $("#availableContacts option:selected").each(function () {
            var optionVal = $(this).val();
            var optionText = $(this).text();

            // Check if the option already exists in the selected list
            if ($("#selectedContacts option[value='" + optionVal + "']").length === 0) {
                $("#selectedContacts").append(new Option(optionText, optionVal, true, true));
                $(this).remove();
            }
        });
        updateSelectedContactsHiddenField();
    });

    $("#btnRemoveContact").click(function () {
        $("#selectedContacts option:selected").each(function () {
            var optionVal = $(this).val();
            var optionText = $(this).text();

            // Add back to available list
            $("#availableContacts").append(new Option(optionText, optionVal, false, false));
            $(this).remove();
        });
        updateSelectedContactsHiddenField();
    });

    // Create a hidden field to store selected contact IDs
    if (!$("#selectedContactIds").length) {
        $("form").append('<input type="hidden" id="selectedContactIds" name="selectedContactIds" value="" />');
    }

    function updateSelectedContactsHiddenField() {
        var selectedIds = [];
        $("#selectedContacts option").each(function () {
            selectedIds.push($(this).val());
        });
        $("#selectedContactIds").val(selectedIds.join(','));
    }

    // Handle contact creation
    $("#saveContactBtn").click(function () {
        if (!$("#createContactForm").valid()) {
            return;
        }

        $.ajax({
            url: '/Contact/CreateContactAjax',
            type: 'POST',
            data: $("#createContactForm").serialize(),
            success: function (response) {
                if (response.success) {
                    // Add the new contact to the selected contacts list directly
                    var newOption = new Option(response.contactName, response.contactId, true, true);
                    $("#selectedContacts").append(newOption);

                    // Update hidden field
                    updateSelectedContactsHiddenField();

                    // Close modal and reset form
                    $("#createContactModal").modal('hide');
                    $("#createContactForm")[0].reset();

                    // Show success message
                    alert('Contact created successfully');
                } else {
                    // Display error message
                    alert(response.message || 'Error creating contact');
                }
            },
            error: function () {
                alert('An error occurred while creating the contact');
            }
        });
    });

    // Ensure selected contacts are included when form is submitted
    $("form").submit(function () {
        updateSelectedContactsHiddenField();
        return true;
    });
});
