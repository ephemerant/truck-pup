## Test Cases Report
### Truck Pup App

Team Intereus

---

| Test Case #: 1 | Test Case Name: HTTP Verb tampering login |
| --- | --- |
| System: Truck Pup App | Subsystem: Login System |
| Designed By: Jason Lytle | Design Date: 4/6/2017 |
| Executed By: Jason Lytle | Latest Execution Date: 4/6/2017 |
| Short Description: | Use Live HTTP headers to login to system |


| Preconditions: |
| --- |
| N/A |


| Step / Attempt | Action | Expected System Response | Pass / Fail | Comment |
| :---: | --- | --- | --- | --- |
| 1 | Use "Post" Command to attempt to bypass login | Failure to login | Fail | Was able to bypass login, but not able to view/save anything |
| 2 | Try "Post" Command again after fix was applied | Failure to login | Pass | "Post" command led back to login screen |


| Post-conditions |
| --- |
| Truck Pup App login system has a section for sending "Post" command, this was altered to not allow "Post" to be sent before logging in |


---

| Test Case #: 2 | Test Case Name: Test flexibility of truck names (length and adding symbols) |
| --- | --- |
| System: Truck Pup App | Subsystem: Truck Name Subsystem |
| Designed By: Jason Lytle | Design Date: 4/25/2017 |
| Executed By: Jason Lytle | Execution Date: 4/25/2017 |
| Short Description: | Create/Edit Truck Names to test limits of length and if symbols can be added |

| Preconditions: |
| --- |
| App has a 125 character limit? |

| Step / Attempt | Action | Expected System Response | Pass / Fail | Comment |
| :---: | --- | --- | --- | --- |
| 1 | Create a long truck name (< 100 characters, but pushes limit) | Pass | Pass |  |
| 2 | Create a longer truck name (>= 100 characters) | Pass | Pass | App blocks more than 100 characters |
| 3 | Create a truck name with some symbols in it | Pass | Pass |  |
| 4 | Create a truck name with all symbols in it | Pass | Pass |  |

| Post-conditions |
| --- |
|  |

---

| Test Case #: 3 | Test Case Name: Test flexibility of truck menu names (length and adding symbols) |
| --- | --- |
| System: Truck Pup App | Subsystem: Truck Menu Names |
| Designed By: Jason Lytle | Design Date: 4/25/2017 |
| Executed By: Jason Lytle | Execution Date: 4/25/2017 |
| Short Description: | Create/Edit Truck Menu names to test limits of length and if symbols can be added |

| Preconditions: |
| --- |
| App has a 125 character limit? |

| Step / Attempt | Action | Expected System Response | Pass / Fail | Comment |
| :---: | --- | --- | --- | --- |
| 1 | Create a long menu item name (< 125 characters, but pushes limit | Pass |  |  |
| 2 | Create a long menu item name (>= 125 characters) | Fail |  |  |
| 3 | Create a menu item name with symbols in it | Pass |  |  |
| 4 | Create a menu item name with all symbols in it | Pass |  |  |

| Post-conditions |
| --- |
|  |

---

# **Template: DO NOT DELETE!!**

| Test Case #:  | Test Case Name:  |
| --- | --- |
| System: Truck Pup App | Subsystem:  |
| Designed By:  | Design Date:  |
| Executed By:  | Execution Date:  |
| Short Description: |  |


| Preconditions: |
| --- |
|  |


| Step / Attempt | Action | Expected System Response | Pass / Fail | Comment |
| :---: | --- | --- | --- | --- |
|  |  |  |  |  |


| Post-conditions |
| --- |
|  |
