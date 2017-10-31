SOURCE_DIR="csv/"
BIN_DIR='../bin'
INPUT_DIR='tmp/input'
OUTPUT_DIR='tmp/output'
FRONT_GDS_DIR=../../../Assets/Script/Utils/GDS/Generated

if [ ! -d "$INPUT_DIR" ]; then 
	mkdir -p "$INPUT_DIR"
fi
if [ ! -d "$OUTPUT_DIR" ]; then 
	mkdir -p "$OUTPUT_DIR"
fi

rm -rf tmp
mkdir tmp
mkdir tmp/input
mkdir tmp/output
cp -r "$SOURCE_DIR" "$INPUT_DIR/"
mono $BIN_DIR/GDSTool.exe --csvPath ../tools/$INPUT_DIR --dstPath ../tools/$OUTPUT_DIR --languages "csharp|golang" --namespaces "GDSKit|main"
rm -rf "$FRONT_GDS_DIR/*"
cp $OUTPUT_DIR/csharp/* $FRONT_GDS_DIR/
