#!/bin/bash
set -e

# Directorio objetivo (por defecto el actual)
target_dir="${1:-.}"
scripts_dir="$target_dir/Assets/Scripts"

# Verificar que existe la carpeta Scripts
if [ ! -d "$scripts_dir" ]; then
    echo "Error: no se encontró el directorio Assets/Scripts en $target_dir."
    echo "Directorio actual: $(pwd)"
    exit 1
fi

# Generar el output
output=$(
    echo "=== ESTRUCTURA DEL PROYECTO UNITY ==="
    echo "--- Raíz ---"
    find "$target_dir" -maxdepth 1 -mindepth 1 ! -name 'Library' ! -name 'Builds' ! -name 'Temp' -printf '%f\n' | sort
    echo
    echo "--- Contenido de Assets/Scripts ---"
    find "$scripts_dir" -type f -name "*.cs" -printf '%P\n' | sort
    echo
    echo "=== CÓDIGO FUENTE C# ==="
    
    # Buscar todos los archivos .cs y mostrar su contenido
    find "$scripts_dir" -type f -name "*.cs" | sort | while read -r file; do
        # Obtener la ruta relativa desde Scripts/
        relative_path="${file#$scripts_dir/}"
        echo "--- $relative_path ---"
        cat "$file"
        echo
    done
)

# Copiar al portapapeles en Windows con Git Bash
if command -v clip >/dev/null 2>&1; then
    # Git Bash en Windows tiene 'clip'
    echo "$output" | clip
    echo "Contenido copiado al portapapeles (Windows clip)."
elif command -v pbcopy >/dev/null 2>&1; then
    # Por si acaso (macOS en Windows via WSL?)
    echo "$output" | pbcopy
    echo "Contenido copiado al portapapeles (pbcopy)."
else
    # Fallback: guardar en archivo
    echo "$output" > unity_code_output.txt
    echo "No se encontró clip. Contenido guardado en unity_code_output.txt"
    echo "Puedes copiarlo manualmente con Ctrl+A, Ctrl+C"
fi

# Mostrar preview
echo
echo "=== PREVIEW (primeras 20 líneas) ==="
echo "$output" | head -n 20
echo "..."
