import re

src = open(r"docs/design/prototype/shell.js", encoding="utf-8").read()

start = src.index("var ICONS = {")
end = src.index("};", start)
body = src[start + len("var ICONS = {"):end]

pattern = re.compile(r"""(?:"([^"]+)"|'([^']+)'|([A-Za-z0-9_-]+))\s*:\s*'((?:[^'\\]|\\.)*)'""", re.S)
icons = {}
for m in pattern.finditer(body):
    key = m.group(1) or m.group(2) or m.group(3)
    val = m.group(4)
    icons[key] = val

print("parsed icons:", len(icons))

def attr(attrs, name):
    m = re.search(rf'{name}="([^"]+)"', attrs)
    return float(m.group(1)) if m else 0.0

def rect_to_path(attrs):
    x = attr(attrs, "x"); y = attr(attrs, "y"); w = attr(attrs, "width"); h = attr(attrs, "height")
    rx = attr(attrs, "rx"); ry = attr(attrs, "ry")
    if ry == 0: ry = rx
    if rx == 0:
        return f"M{x},{y} L{x+w},{y} L{x+w},{y+h} L{x},{y+h} Z"
    r = min(rx if rx else ry, w/2, h/2)
    return (f"M{x+r},{y} L{x+w-r},{y} Q{x+w},{y} {x+w},{y+r} "
            f"L{x+w},{y+h-r} Q{x+w},{y+h} {x+w-r},{y+h} "
            f"L{x+r},{y+h} Q{x},{y+h} {x},{y+h-r} "
            f"L{x},{y+r} Q{x},{y} {x+r},{y} Z")

def circle_to_path(cx, cy, r):
    return (f"M{cx-r},{cy} A{r},{r} 0 1 0 {cx+r},{cy} "
            f"A{r},{r} 0 1 0 {cx-r},{cy} Z")

def ellipse_to_path(cx, cy, rx, ry):
    return (f"M{cx-rx},{cy} A{rx},{ry} 0 1 0 {cx+rx},{cy} "
            f"A{rx},{ry} 0 1 0 {cx-rx},{cy} Z")

def convert_markup(markup):
    parts = []
    for tag in re.finditer(r'<(\w+)([^>]*)/?>', markup):
        name = tag.group(1)
        attrs = tag.group(2)
        if name == "path":
            m = re.search(r'd="([^"]+)"', attrs)
            if m:
                parts.append(m.group(1))
        elif name == "rect":
            parts.append(rect_to_path(attrs))
        elif name == "circle":
            cx = attr(attrs, "cx"); cy = attr(attrs, "cy"); r = attr(attrs, "r")
            parts.append(circle_to_path(cx, cy, r))
        elif name == "ellipse":
            cx = attr(attrs, "cx"); cy = attr(attrs, "cy")
            rx = attr(attrs, "rx"); ry = attr(attrs, "ry")
            parts.append(ellipse_to_path(cx, cy, rx, ry))
        elif name == "line":
            x1 = attr(attrs, "x1"); y1 = attr(attrs, "y1")
            x2 = attr(attrs, "x2"); y2 = attr(attrs, "y2")
            parts.append(f"M{x1},{y1} L{x2},{y2}")
    return " ".join(parts)

def to_key(name):
    s = name.strip().strip('"').strip("'").replace("-", "_")
    parts = re.split(r'[_\s]+', s)
    return "Icon" + "".join(p[:1].upper() + p[1:] for p in parts if p)

lines = []
lines.append('<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"')
lines.append('                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">')
lines.append('')
lines.append('    <!-- Iconos del prototipo: 24x24 outline, trazo 1.75 -->')
lines.append('')

for name, markup in icons.items():
    key = to_key(name)
    d = convert_markup(markup)
    lines.append(f'    <PathGeometry x:Key="{key}" Figures="{d}" />')

lines.append('')
lines.append('</ResourceDictionary>')

out = "\n".join(lines)
open(r"src/SOK.UI/Styles/Icons.xaml", "w", encoding="utf-8").write(out)
print("written", len(icons), "icons")
print("keys sample:", list(icons.keys())[:8])