Purpose
-------
Reporting project produces HTML/other report formats from scan results.

Removed folders
- Html/
- Models/
- Pdf/
- Templates/

If you need to add report assets, create the folders and add generator code under a Reporting or Generators namespace. Prefer streaming large reports and separating generation from persistence.

Suggested structure
- Generators/
- Templates/
- Models/

Notes
- Avoid committing empty folders; add README or sample files when creating new folders.
