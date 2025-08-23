########
#
#	Tree-Sitter Base and Common Language Parsers
#

CFLAGS=-g -O0 -fPIC -Itree-sitter/lib/include
LFLAGS=-shared

BIN=dist

# Detect platform and set library extension
UNAME_S := $(shell uname -s)
ifeq ($(UNAME_S),Linux)
    LIB_EXT=.so
endif
ifeq ($(UNAME_S),Darwin)
    LIB_EXT=.dylib
endif
ifeq ($(OS),Windows_NT)
    LIB_EXT=.dll
endif
ifndef LIB_EXT
    LIB_EXT=.so
endif

LIBS=\
	$(BIN)/libtree-sitter$(LIB_EXT) \
	$(BIN)/libtree-sitter-typescript$(LIB_EXT) \

all: dirs $(LIBS)

dirs:
	@if [ ! -f tree-sitter/lib/src/alloc.c ]; then echo ""; fi
	@if [ ! -f tree-sitter/lib/src/alloc.c ]; then echo "*** Error: Missing tree-sitter sources.  Did you use --recursive with git clone?"; fi
	@if [ ! -f tree-sitter/lib/src/alloc.c ]; then echo ""; fi
	@if [ ! -d $(BIN) ]; then mkdir -p $(BIN); fi

########
#
# 	Tree-Sitter Base Library
#
$(BIN)/tree-sitter.obj: \
		tree-sitter/lib/src/alloc.c \
		tree-sitter/lib/src/get_changed_ranges.c \
		tree-sitter/lib/src/language.c \
		tree-sitter/lib/src/lexer.c \
		tree-sitter/lib/src/lib.c \
		tree-sitter/lib/src/node.c \
		tree-sitter/lib/src/parser.c \
		tree-sitter/lib/src/query.c \
		tree-sitter/lib/src/stack.c \
		tree-sitter/lib/src/subtree.c \
		tree-sitter/lib/src/tree.c \
		tree-sitter/lib/src/tree_cursor.c
	gcc $(CFLAGS) -o $@ \
		-Itree-sitter/lib/src -Itree-sitter/lib/src/unicode \
		-c tree-sitter/lib/src/lib.c

$(BIN)/libtree-sitter$(LIB_EXT): $(BIN)/tree-sitter.obj
	gcc $(LFLAGS) $(CFLAGS) -o $@ $^


########
#
#   TypeScript
#
$(BIN)/tree-sitter-typescript-parser.obj: tree-sitter-typescript/typescript/src/parser.c
	gcc $(CFLAGS) -o $@ -Itree-sitter-typescript/typescript/src -c $^

$(BIN)/tree-sitter-typescript-scanner.obj: tree-sitter-typescript/typescript/src/scanner.c
	gcc $(CFLAGS) -o $@ -Itree-sitter-typescript/typescript/src -c $^

$(BIN)/libtree-sitter-typescript$(LIB_EXT): $(BIN)/tree-sitter-typescript-parser.obj $(BIN)/tree-sitter-typescript-scanner.obj
	gcc $(LFLAGS) $(CFLAGS) -o $@ $^

########
#
#	Clean
#
clean:
	-rm -f *.obj $(BIN)/*.obj 2>/dev/null
	-rm -f $(BIN)/libtree-sitter*.dll $(BIN)/libtree-sitter*.so $(BIN)/libtree-sitter*.dylib 2>/dev/null
	-rm -f $(BIN)/tree-sitter*.exp $(BIN)/tree-sitter*.lib $(BIN)/tree-sitter*.pdb 2>/dev/null
	-rm -f libtree-sitter*.dll libtree-sitter*.so libtree-sitter*.dylib 2>/dev/null
	-rm -f tree-sitter*.exp tree-sitter*.lib tree-sitter*.pdb 2>/dev/null
	-rm -f $(BIN)/xred.* $(BIN)/test $(BIN)/test.exp $(BIN)/test.pdb $(BIN)/test.lib 2>/dev/null
	-rm -f *~ 2>/dev/null
	@echo
	@echo

########
#
#	
#

xt:
	xred -tree xpdb.cs > xpdb.tree && emacs xpdb.tree
	xred -tree vm_devices_net_netvsp_src_lib.rs > lib.tree && emacs lib.tree
	xred -tree minkernel_fs_ntfs_create.c > create.tree && emacs create.tree
	xred -tree dpx.cpp > dpx.tree && emacs dpx.tree

# End of File.
