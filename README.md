It's supposed that json can have 3 types: array, object, primitive type. Based on this I tried to develop general version for json mapping to HTML template.<br />
PS More tests are needed to the project
<br />
Class library named **Generator** that appears to generate HTML content based on a template and JSON data. This class uses regular expressions and string manipulation to process loops, objects, and primitive types in the template.
<br />
Here's a brief overview of the key functionalities:
<br />
**FindStartLoopIndexes** and **FindEndLoopIndexes**:
<br />
**FindStartLoopIndexes**: Uses a regular expression to find the starting indexes of for-loops in the template.<br />
**FindEndLoopIndexes**: Finds the indexes of "{% endfor %}" in the template.<br />
**GetLoopBoundaries**: Determines the boundaries of for-loops by matching start and end indexes and extracts relevant information.<br />
**ProcessTemplate**: Takes the root JSON object and processes the template by handling arrays, objects, and primitive types.<br />

ProcessLoop, ProcessObject, ProcessPrimitiveType:<br />

**ProcessLoop**: Replaces the for-loop placeholder with content based on each item in the array.<br />
**ProcessObject**: Replaces placeholders within the template with values from the JSON object.<br />
**ProcessPrimitiveType**: Replaces placeholders for primitive types.<br />
**ApplyType**: Applies specific types (such as "price" or "paragraph") to the input values based on the provided typeName.
