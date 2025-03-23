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

// Handle profile photo upload
function uploadProfilePhoto(memberId) {
    const fileInput = document.getElementById('photoUpload');
    if (fileInput.files && fileInput.files[0]) {
        // Show loading indicator
        const overlay = document.querySelector('.profile-image-overlay');
        const originalContent = overlay.innerHTML;
        overlay.innerHTML = '<div class="spinner-border text-light" role="status"><span class="visually-hidden">Loading...</span></div>';

        const formData = new FormData();
        formData.append('photo', fileInput.files[0]);
        formData.append('memberId', memberId);

        fetch('/Member/UpdateProfilePhoto', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error('Network response was not ok');
            })
            .then(data => {
                if (data.success) {
                    // Reload the page to show the new image
                    window.location.reload();
                } else {
                    // Restore original content and show error
                    overlay.innerHTML = originalContent;
                    alert('Error uploading photo: ' + data.message);
                }
            })
            .catch(error => {
                // Restore original content and show error
                overlay.innerHTML = originalContent;
                console.error('Error:', error);
                alert('Error uploading photo. Please try again.');
            });
    }
}

// Initialize tooltips
document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Add event listener for file input change
    const photoUpload = document.getElementById('photoUpload');
    if (photoUpload) {
        photoUpload.addEventListener('change', function () {
            if (this.files && this.files[0]) {
                // Preview image (optional)
                const reader = new FileReader();
                reader.onload = function (e) {
                    const profileImage = document.querySelector('.profile-image');
                    if (profileImage) {
                        profileImage.src = e.target.result;
                    }
                }
                reader.readAsDataURL(this.files[0]);

                // Auto upload
                const memberId = this.getAttribute('data-member-id');
                if (memberId) {
                    uploadProfilePhoto(memberId);
                }
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
});