$(document).ready(function () {
    // Add event listener for file input change
    const photoUpload = document.getElementById('photoUpload');
    if (photoUpload) {
        photoUpload.addEventListener('change', function () {
            if (this.files && this.files[0]) {
                // Preview image
                const reader = new FileReader();
                reader.onload = function (e) {
                    const profileImage = document.querySelector('.profile-image');
                    if (profileImage) {
                        profileImage.src = e.target.result;
                    }
                }
                reader.readAsDataURL(this.files[0]);

                // Upload the photo
                uploadProfilePhoto(this.getAttribute('data-member-id'), this.files[0]);
            }
        });
    }

    // Dismiss alerts automatically after 5 seconds
    setTimeout(function () {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(function (alert) {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        });
    }, 5000);

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

function uploadProfilePhoto(memberId, file) {
    // Show loading indicator
    const statusDiv = document.getElementById('uploadStatus');
    statusDiv.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';
    statusDiv.style.display = 'block';

    const formData = new FormData();
    formData.append('thePicture', file);
    formData.append('id', memberId);

    $.ajax({
        url: '@Url.Action("UpdatePhoto", "Member")',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.success) {
                statusDiv.className = 'alert alert-success mt-2 text-center';
                statusDiv.innerHTML = 'Photo uploaded successfully!';

                // Reload the page after a short delay to show the new image
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                statusDiv.className = 'alert alert-danger mt-2 text-center';
                statusDiv.innerHTML = 'Error: ' + result.message;
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            statusDiv.className = 'alert alert-danger mt-2 text-center';
            statusDiv.innerHTML = 'Error uploading photo. Please try again.';
        }
    });
}

// Toggle text for read more/less functionality
function toggleText() {
    var moreText = document.getElementById("moreText");
    var readMore = document.getElementById("readMore");

    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = " Read More";
    }
}

// Toggle text for member name with specific item ID
function toggleTextName(itemId) {
    var moreText = document.getElementById("moreTextName" + itemId);
    var readMore = document.getElementById("readMoreName" + itemId);

    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = " Read More";
    }
}

// Toggle text for city with specific item ID
function toggleTextCity(itemId) {
    var moreText = document.getElementById("moreTextCity" + itemId);
    var readMore = document.getElementById("readMoreCity" + itemId);

    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = " Read More";
    }
}

// Toggle text for industry with specific item ID
function toggleTextIndustry(itemId) {
    var moreText = document.getElementById("moreTextIndustry" + itemId);
    var readMore = document.getElementById("readMoreIndustry" + itemId);

    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = " Read More";
    }
}

// Toggle industry display and count
function toggleIndustry(id) {
    var countSpan = document.getElementById("industryCount" + id);
    var collapseElement = document.getElementById("collapseIndustry" + id);

    if (collapseElement.classList.contains("show")) {
        countSpan.style.display = "inline"; // Show the count when collapsed
    } else {
        countSpan.style.display = "none"; // Hide the count when expanded
    }
}