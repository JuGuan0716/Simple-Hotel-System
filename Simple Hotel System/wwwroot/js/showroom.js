let bookedRanges = [];
let checkInPicker;
let checkOutPicker;

document.addEventListener('DOMContentLoaded', function () {

    const checkInInput = document.querySelector('input[name="CheckIn"]');
    const checkOutInput = document.querySelector('input[name="CheckOut"]');
    const priceMinInput = document.getElementById("priceMin");
    const priceMaxInput = document.getElementById("priceMax");
    const typeSelect = document.getElementById("roomTypeSelect");
    const nameSelect = document.getElementById("roomNameSelect");

    if (typeof rooms === 'undefined') return;


    // Initialize Flatpickr
    checkInPicker = flatpickr(checkInInput, {
        dateFormat: "Y-m-d",
        minDate: new Date().fp_incr(1),
        disable: [],
        onChange: function (selectedDates) {
            if (selectedDates.length > 0) {
                checkOutPicker.set('minDate', selectedDates[0].fp_incr(1));
                checkOutPicker.clear();
            }
        }
    });

    checkOutPicker = flatpickr(checkOutInput, {
        dateFormat: "Y-m-d",
        minDate: new Date().fp_incr(2),
        disable: [],
        onChange: function (selectedDates) {

            if (!checkInPicker.selectedDates.length) return;

            let checkInDate = checkInPicker.selectedDates[0];
            let checkOutDate = selectedDates[0];

            let overlap = bookedRanges.some(range => {
                let start = new Date(range.from);
                let end = new Date(range.to);
                return checkInDate <= end && checkOutDate >= start;
            });

            if (overlap) {
                alert("Selected dates overlap with existing booking.");
                checkOutPicker.clear();
            }
        }
    });


    // FILTER LOGIC
    function getFilteredRooms() {
        let min = parseFloat(priceMinInput.value) || 0;
        let max = parseFloat(priceMaxInput.value) || Infinity;

        return rooms.filter(r =>
            r.Status === "Available" &&
            r.Price >= min &&
            r.Price <= max
        );
    }

    function rebuildTypeDropdown(filteredRooms) {
        typeSelect.innerHTML =
            '<option value="" selected disabled>Select Room Type</option>';

        let uniqueTypes = [...new Set(filteredRooms.map(r => r.Type))];

        uniqueTypes.forEach(type => {
            let option = document.createElement("option");
            option.value = type;
            option.textContent = type;
            typeSelect.appendChild(option);
        });

        nameSelect.innerHTML =
            '<option value="" selected disabled>Select Room Name</option>';
    }

    function rebuildNameDropdown(filteredRooms, selectedType) {
        nameSelect.innerHTML =
            '<option value="" selected disabled>Select Room Name</option>';

        let roomsByType = filteredRooms.filter(r => r.Type === selectedType);

        roomsByType.forEach(room => {
            let option = document.createElement("option");
            option.value = room.Id;
            option.textContent = room.Name;
            nameSelect.appendChild(option);
        });
    }

    function filterRooms() {
        let filtered = getFilteredRooms();
        rebuildTypeDropdown(filtered);
    }

    if (priceMinInput && priceMaxInput) {
        priceMinInput.addEventListener('input', filterRooms);
        priceMaxInput.addEventListener('input', filterRooms);
    }


    // TYPE CHANGE
    typeSelect.addEventListener('change', function () {
        let selectedType = this.value;
        let filtered = getFilteredRooms();

        if (!selectedType) {
            nameSelect.innerHTML =
                '<option value="">Select Room Name</option>';
            return;
        }

        rebuildNameDropdown(filtered, selectedType);
    });


    // ROOM NAME CHANGE
    nameSelect.addEventListener('change', function () {

        let selectedRoomId = parseInt(this.value);
        if (!selectedRoomId) return;

        let selectedRoom = rooms.find(r => r.Id === selectedRoomId);
        if (!selectedRoom) return;

        // Update UI
        document.getElementById("hiddenRoomId").value = selectedRoom.Id;
        document.getElementById("roomName").innerText = selectedRoom.Name;
        document.getElementById("roomType").innerText = selectedRoom.Type;
        document.getElementById("roomPrice").innerText =
            "RM " + selectedRoom.Price + " / Night";
        document.getElementById("roomImage").src =
            selectedRoom.PicUrl ? selectedRoom.PicUrl : "/images/no-image.png";

        // Fetch booked dates
        fetch('/Booking/GetBookedDates?roomId=' + selectedRoomId)
            .then(response => response.json())
            .then(data => {

                bookedRanges = data;

                checkInPicker.clear();
                checkOutPicker.clear();

                checkInPicker.set('disable', bookedRanges);
                checkOutPicker.set('disable', bookedRanges);
            })
            .catch(error => {
                console.error("Error loading booked dates:", error);
            });
    });

    // INITIAL LOAD
    filterRooms();
});
