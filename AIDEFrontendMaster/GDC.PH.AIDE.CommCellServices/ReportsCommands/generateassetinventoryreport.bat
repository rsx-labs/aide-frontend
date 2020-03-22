@echo off
set "_names=%~1"
"C:\Program Files (x86)\kool-aide\kool-aide" generate-report -r asset-inventory --format excel --output "C:\GeneratedReports\Retail Services DEV Hardware Inventory.xlsx" --autorun  --params {\"sorts\":[\"EmployeeName\",\"Description\",\"Manufacturer\",\"Model\"],\"divisions\":[1],\"ids\":[%_names%],\"status\":[%2]}

