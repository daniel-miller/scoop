// SCORM 1.2 API Implementation
function ScormAPI(packageNumber, learnerId) {
    this.packageNumber = packageNumber;
    this.learnerId = learnerId;
    this.initialized = false;
    this.finished = false;
    this.lastError = "0";

    // Data model
    this.dataModel = {
        "cmi.core.student_id": learnerId,
        "cmi.core.student_name": "Student, Test",
        "cmi.core.lesson_location": "",
        "cmi.core.credit": "credit",
        "cmi.core.lesson_status": "not attempted",
        "cmi.core.entry": "ab-initio",
        "cmi.core.score.raw": "",
        "cmi.core.score.max": "",
        "cmi.core.score.min": "",
        "cmi.core.total_time": "0000:00:00",
        "cmi.core.lesson_mode": "normal",
        "cmi.core.exit": "",
        "cmi.core.session_time": "0000:00:00",
        "cmi.launch_data": "",
        "cmi.comments": "",
        "cmi.comments_from_lms": "",
        "cmi.objectives._count": "0",
        "cmi.student_data.mastery_score": "",
        "cmi.student_data.max_time_allowed": "",
        "cmi.student_data.time_limit_action": "",
        "cmi.student_preference.audio": "0",
        "cmi.student_preference.language": "",
        "cmi.student_preference.speed": "0",
        "cmi.student_preference.text": "0",
        "cmi.interactions._count": "0",
        "cmi.suspend_data": ""
    };

    // Load existing data from server
    this.loadData();
}

// SCORM 1.2 API Methods
ScormAPI.prototype.LMSInitialize = function (param) {
    console.log("LMSInitialize called");

    if (this.initialized) {
        this.lastError = "101"; // Already initialized
        return "false";
    }

    if (param !== "") {
        this.lastError = "201"; // Invalid argument
        return "false";
    }

    this.initialized = true;
    this.lastError = "0";

    // Update UI
    this.updateStatus("incomplete");

    return "true";
};

ScormAPI.prototype.LMSFinish = function (param) {
    console.log("LMSFinish called");

    if (!this.initialized) {
        this.lastError = "301"; // Not initialized
        return "false";
    }

    if (this.finished) {
        this.lastError = "103"; // Already finished
        return "false";
    }

    if (param !== "") {
        this.lastError = "201"; // Invalid argument
        return "false";
    }

    // Save data to server
    this.saveData();

    this.finished = true;
    this.initialized = false;
    this.lastError = "0";

    return "true";
};

ScormAPI.prototype.LMSGetValue = function (element) {
    console.log("LMSGetValue: " + element);

    if (!this.initialized) {
        this.lastError = "301"; // Not initialized
        return "";
    }

    if (this.finished) {
        this.lastError = "103"; // Already finished
        return "";
    }

    // Check if element exists in data model
    if (element in this.dataModel) {
        this.lastError = "0";
        return this.dataModel[element];
    }

    // Handle dynamic elements (objectives, interactions)
    if (element.indexOf("cmi.objectives.") === 0 ||
        element.indexOf("cmi.interactions.") === 0) {
        // Would need to implement dynamic element handling
        this.lastError = "0";
        return "";
    }

    this.lastError = "401"; // Undefined data model element
    return "";
};

ScormAPI.prototype.LMSSetValue = function (element, value) {
    console.log("LMSSetValue: " + element + " = " + value);

    if (!this.initialized) {
        this.lastError = "301"; // Not initialized
        return "false";
    }

    if (this.finished) {
        this.lastError = "103"; // Already finished
        return "false";
    }

    // Validate and set value based on element
    switch (element) {
        case "cmi.core.lesson_status":
            if (["passed", "completed", "failed", "incomplete", "browsed", "not attempted"].indexOf(value) === -1) {
                this.lastError = "405"; // Incorrect data type
                return "false";
            }
            this.dataModel[element] = value;
            this.updateStatus(value);
            break;

        case "cmi.core.score.raw":
        case "cmi.core.score.min":
        case "cmi.core.score.max":
            if (isNaN(value)) {
                this.lastError = "405"; // Incorrect data type
                return "false";
            }
            this.dataModel[element] = value;
            this.updateScore();
            break;

        case "cmi.core.exit":
            if (["time-out", "suspend", "logout", ""].indexOf(value) === -1) {
                this.lastError = "405"; // Incorrect data type
                return "false";
            }
            this.dataModel[element] = value;
            break;

        case "cmi.core.session_time":
            // Validate time format (HHHH:MM:SS.SS)
            if (!this.isValidTime(value)) {
                this.lastError = "405"; // Incorrect data type
                return "false";
            }
            this.dataModel[element] = value;
            this.updateTotalTime(value);
            break;

        case "cmi.core.lesson_location":
        case "cmi.suspend_data":
        case "cmi.comments":
            this.dataModel[element] = value;
            break;

        case "cmi.core.student_id":
        case "cmi.core.student_name":
        case "cmi.core.credit":
        case "cmi.core.entry":
        case "cmi.core.total_time":
        case "cmi.core.lesson_mode":
            this.lastError = "403"; // Read only
            return "false";

        default:
            // Handle dynamic elements
            if (element.indexOf("cmi.objectives.") === 0 ||
                element.indexOf("cmi.interactions.") === 0) {
                // Would implement dynamic element handling
                this.dataModel[element] = value;
                this.lastError = "0";
                return "true";
            }

            this.lastError = "401"; // Undefined data model element
            return "false";
    }

    this.lastError = "0";

    // Auto-save every few sets
    if (Math.random() < 0.3) {
        this.saveData();
    }

    return "true";
};

ScormAPI.prototype.LMSCommit = function (param) {
    console.log("LMSCommit called");

    if (!this.initialized) {
        this.lastError = "301"; // Not initialized
        return "false";
    }

    if (this.finished) {
        this.lastError = "103"; // Already finished
        return "false";
    }

    if (param !== "") {
        this.lastError = "201"; // Invalid argument
        return "false";
    }

    // Save data to server
    this.saveData();

    this.lastError = "0";
    return "true";
};

ScormAPI.prototype.LMSGetLastError = function () {
    return this.lastError;
};

ScormAPI.prototype.LMSGetErrorString = function (errorCode) {
    var errors = {
        "0": "No error",
        "101": "General exception",
        "102": "General initialization failure",
        "103": "Already terminated",
        "104": "Content instance terminated",
        "111": "General termination failure",
        "112": "Termination before initialization",
        "113": "Termination after termination",
        "122": "Retrieve data before initialization",
        "123": "Retrieve data after termination",
        "132": "Store data before initialization",
        "133": "Store data after termination",
        "142": "Commit before initialization",
        "143": "Commit after termination",
        "201": "General argument error",
        "301": "General get failure",
        "351": "General set failure",
        "391": "General commit failure",
        "401": "Undefined data model element",
        "402": "Unimplemented data model element",
        "403": "Data model element value not initialized",
        "404": "Data model element is read only",
        "405": "Data model element is write only",
        "406": "Data model element type mismatch",
        "407": "Data model element value out of range",
        "408": "Data model dependency not established"
    };

    return errors[errorCode] || "Unknown error";
};

ScormAPI.prototype.LMSGetDiagnostic = function (errorCode) {
    return this.LMSGetErrorString(errorCode);
};

// Helper methods
ScormAPI.prototype.isValidTime = function (time) {
    var pattern = /^\d{2,4}:\d{2}:\d{2}(\.\d{1,2})?$/;
    return pattern.test(time);
};

ScormAPI.prototype.updateTotalTime = function (sessionTime) {
    // Add session time to total time
    var total = this.dataModel["cmi.core.total_time"];
    var session = sessionTime;

    // Parse times (HHHH:MM:SS.SS format)
    var totalParts = total.split(':');
    var sessionParts = session.split(':');

    var totalSeconds = parseInt(totalParts[0]) * 3600 + parseInt(totalParts[1]) * 60 + parseFloat(totalParts[2]);
    var sessionSeconds = parseInt(sessionParts[0]) * 3600 + parseInt(sessionParts[1]) * 60 + parseFloat(sessionParts[2]);

    var newTotal = totalSeconds + sessionSeconds;

    var hours = Math.floor(newTotal / 3600);
    var minutes = Math.floor((newTotal % 3600) / 60);
    var seconds = newTotal % 60;

    this.dataModel["cmi.core.total_time"] =
        String(hours).padStart(4, '0') + ':' +
        String(minutes).padStart(2, '0') + ':' +
        String(seconds.toFixed(2)).padStart(5, '0');
};

ScormAPI.prototype.updateStatus = function (status) {
    var statusElement = document.getElementById('lessonStatus');
    if (statusElement) {
        statusElement.textContent = status;
    }

    var indicator = document.getElementById('statusIndicator');
    if (indicator) {
        if (status === 'completed' || status === 'passed') {
            indicator.classList.add('active');
        } else {
            indicator.classList.remove('active');
        }
    }
};

ScormAPI.prototype.updateScore = function () {
    var raw = this.dataModel["cmi.core.score.raw"];
    var max = this.dataModel["cmi.core.score.max"];

    if (raw && max) {
        var percentage = Math.round((parseFloat(raw) / parseFloat(max)) * 100);

        var scoreDisplay = document.getElementById('scoreDisplay');
        if (scoreDisplay) {
            scoreDisplay.textContent = percentage;
        }

        var progressBar = document.getElementById('progressBar');
        if (progressBar) {
            progressBar.style.width = percentage + '%';
        }
    }
};

ScormAPI.prototype.loadData = function () {
    var self = this;
    var xhr = new XMLHttpRequest();
    var url = appRoot + 'sco-progress?action=load&packageNumber=' + this.packageNumber + '&learnerId=' + this.learnerId;
    xhr.open('GET', url, false);
    xhr.send();

    if (xhr.status === 200) {
        try {
            var data = JSON.parse(xhr.responseText);
            for (var key in data) {
                if (data.hasOwnProperty(key) && data[key]) {
                    self.dataModel[key] = data[key];
                }
            }
            console.log('SCORM data loaded successfully');
        } catch (e) {
            console.error('Error loading SCORM data:', e);
        }
    } else {
        console.error('Failed to load SCORM data. Status:', xhr.status);
    }
};

ScormAPI.prototype.saveData = function () {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', appRoot + 'sco-progress?action=save&packageNumber=' + this.packageNumber + '&learnerId=' + this.learnerId, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                console.log('SCORM data saved successfully');
            } else {
                console.error('Failed to save SCORM data. Status:', xhr.status);
            }
        }
    };

    xhr.send(JSON.stringify(this.dataModel));
};