using System;

namespace DefMonRelocator {
    internal abstract class Program {
        private static int _addDest;
        private static int _pc;
        private static int _diffRelocate;
        private static byte[] _file;
        private static byte _addFb;
        private static byte _addFc;
        private static byte _add96;

        private static readonly short[] TabFb =
            { 0x16b1, 0x16b5, 0x16bf, 0x16c7, 0x16d1, 0x16dd, 0x16e9, 0x16fd, 0x1709, 0x1714, 0x171e, 0x172c, 0x1734, 0x1747, 0x1758, 0x1763, 0x176e, 0x1771 };

        private static readonly short[] TabFc = { 0x12fd, 0x132e, 0x135f, 0x1390, 0x13c1, 0x13f2 };
        private static readonly short[] Tab96 = { 0x16ba, 0x16cc, 0x16d6, 0x16e2, 0x16f6, 0x1702, 0x1719, 0x1723, 0x174f };

        // ========================================================================================
        private struct OpCode {
            public string Txt;
            public byte Code;
            public byte ArgsNr;
            public bool Relocate;
        }

        // ========================================================================================
        private static readonly OpCode[] Opcodes = new[] {
            new OpCode { Txt = "adc", Code = 0x6d, ArgsNr = 2, Relocate = true }, // ADC $hhll
            new OpCode { Txt = "adc", Code = 0x7d, ArgsNr = 2, Relocate = true }, // ADC $hhll, X
            new OpCode { Txt = "adc", Code = 0x79, ArgsNr = 2, Relocate = true }, // ADC $hhll, Y
            new OpCode { Txt = "adc", Code = 0x65, ArgsNr = 1, Relocate = false }, // ADC $ll
            new OpCode { Txt = "adc", Code = 0x75, ArgsNr = 1, Relocate = false }, // ADC $ll, X
            new OpCode { Txt = "adc", Code = 0x71, ArgsNr = 1, Relocate = false }, // ADC ($ll), Y
            new OpCode { Txt = "adc", Code = 0x61, ArgsNr = 1, Relocate = false }, // ADC ($ll,X)
            new OpCode { Txt = "adc", Code = 0x69, ArgsNr = 1, Relocate = false }, // ADC #$nn
            new OpCode { Txt = "and", Code = 0x2d, ArgsNr = 2, Relocate = true }, // AND $hhll
            new OpCode { Txt = "and", Code = 0x3d, ArgsNr = 2, Relocate = true }, // AND $hhll, X
            new OpCode { Txt = "and", Code = 0x39, ArgsNr = 2, Relocate = true }, // AND $hhll, Y
            new OpCode { Txt = "and", Code = 0x25, ArgsNr = 1, Relocate = false }, // AND $ll
            new OpCode { Txt = "and", Code = 0x35, ArgsNr = 1, Relocate = false }, // AND $ll, X
            new OpCode { Txt = "and", Code = 0x31, ArgsNr = 1, Relocate = false }, // AND ($ll), Y
            new OpCode { Txt = "and", Code = 0x21, ArgsNr = 1, Relocate = false }, // AND ($ll,X)
            new OpCode { Txt = "and", Code = 0x29, ArgsNr = 1, Relocate = false }, // AND #$nn
            new OpCode { Txt = "asl", Code = 0x0a, ArgsNr = 0, Relocate = false }, // ASL
            new OpCode { Txt = "asl", Code = 0x0e, ArgsNr = 2, Relocate = true }, // ASL $hhll
            new OpCode { Txt = "asl", Code = 0x1e, ArgsNr = 2, Relocate = true }, // ASL $hhll, X
            new OpCode { Txt = "asl", Code = 0x06, ArgsNr = 1, Relocate = false }, // ASL $ll
            new OpCode { Txt = "asl", Code = 0x16, ArgsNr = 1, Relocate = false }, // ASL $ll, X
            new OpCode { Txt = "bcc", Code = 0x90, ArgsNr = 1, Relocate = false }, // BCC $hhll
            new OpCode { Txt = "bcs", Code = 0xB0, ArgsNr = 1, Relocate = false }, // BCS $hhll
            new OpCode { Txt = "beq", Code = 0xF0, ArgsNr = 1, Relocate = false }, // BEQ $hhll
            new OpCode { Txt = "bit", Code = 0x2c, ArgsNr = 2, Relocate = true }, // BIT $hhll
            new OpCode { Txt = "bit", Code = 0x24, ArgsNr = 1, Relocate = false }, // BIT $ll
            new OpCode { Txt = "bmi", Code = 0x30, ArgsNr = 1, Relocate = false }, // BMI $hhll
            new OpCode { Txt = "bne", Code = 0xD0, ArgsNr = 1, Relocate = false }, // BNE $hhll
            new OpCode { Txt = "bpl", Code = 0x10, ArgsNr = 1, Relocate = false }, // BPL $hhll
            new OpCode { Txt = "brk", Code = 0x00, ArgsNr = 0, Relocate = false }, // BRK
            new OpCode { Txt = "bvc", Code = 0x50, ArgsNr = 1, Relocate = false }, // BVC $hhll
            new OpCode { Txt = "bvs", Code = 0x70, ArgsNr = 1, Relocate = false }, // BVS $hhll
            new OpCode { Txt = "clc", Code = 0x18, ArgsNr = 0, Relocate = false }, // CLC
            new OpCode { Txt = "cld", Code = 0xD8, ArgsNr = 0, Relocate = false }, // CLD
            new OpCode { Txt = "cli", Code = 0x58, ArgsNr = 0, Relocate = false }, // CLI
            new OpCode { Txt = "clv", Code = 0xB8, ArgsNr = 0, Relocate = false }, // CLV
            new OpCode { Txt = "cmp", Code = 0xCD, ArgsNr = 2, Relocate = true }, // CMP $hhll
            new OpCode { Txt = "cmp", Code = 0xDD, ArgsNr = 2, Relocate = true }, // CMP $hhll, X
            new OpCode { Txt = "cmp", Code = 0xD9, ArgsNr = 2, Relocate = true }, // CMP $hhll, Y
            new OpCode { Txt = "cmp", Code = 0xC5, ArgsNr = 1, Relocate = false }, // CMP $ll
            new OpCode { Txt = "cmp", Code = 0xD5, ArgsNr = 1, Relocate = false }, // CMP $ll, X
            new OpCode { Txt = "cmp", Code = 0xD1, ArgsNr = 1, Relocate = false }, // CMP ($ll), Y
            new OpCode { Txt = "cmp", Code = 0xC1, ArgsNr = 1, Relocate = false }, // CMP ($ll,X)
            new OpCode { Txt = "cmp", Code = 0xC9, ArgsNr = 1, Relocate = false }, // CMP #$nn
            new OpCode { Txt = "cpx", Code = 0xEC, ArgsNr = 2, Relocate = true }, // CPX $hhll
            new OpCode { Txt = "cpx", Code = 0xE4, ArgsNr = 1, Relocate = false }, // CPX $ll
            new OpCode { Txt = "cpx", Code = 0xE0, ArgsNr = 1, Relocate = false }, // CPX #$nn
            new OpCode { Txt = "cpy", Code = 0xCC, ArgsNr = 2, Relocate = true }, // CPY $hhll
            new OpCode { Txt = "cpy", Code = 0xC4, ArgsNr = 1, Relocate = false }, // CPY $ll
            new OpCode { Txt = "cpy", Code = 0xC0, ArgsNr = 1, Relocate = false }, // CPY #$nn
            new OpCode { Txt = "dec", Code = 0xCE, ArgsNr = 2, Relocate = true }, // DEC $hhll
            new OpCode { Txt = "dec", Code = 0xDE, ArgsNr = 2, Relocate = true }, // DEC $hhll, X
            new OpCode { Txt = "dec", Code = 0xC6, ArgsNr = 1, Relocate = false }, // DEC $ll
            new OpCode { Txt = "dec", Code = 0xD6, ArgsNr = 1, Relocate = false }, // DEC $ll, X
            new OpCode { Txt = "dex", Code = 0xCA, ArgsNr = 0, Relocate = false }, // DEX
            new OpCode { Txt = "dey", Code = 0x88, ArgsNr = 0, Relocate = false }, // DEY
            new OpCode { Txt = "eor", Code = 0x4D, ArgsNr = 2, Relocate = true }, // EOR $hhll
            new OpCode { Txt = "eor", Code = 0x5D, ArgsNr = 2, Relocate = true }, // EOR $hhll, X
            new OpCode { Txt = "eor", Code = 0x59, ArgsNr = 2, Relocate = true }, // EOR $hhll, Y
            new OpCode { Txt = "eor", Code = 0x45, ArgsNr = 1, Relocate = false }, // EOR $ll
            new OpCode { Txt = "eor", Code = 0x55, ArgsNr = 1, Relocate = false }, // EOR $ll, X
            new OpCode { Txt = "eor", Code = 0x51, ArgsNr = 1, Relocate = false }, // EOR ($ll), Y
            new OpCode { Txt = "eor", Code = 0x41, ArgsNr = 1, Relocate = false }, // EOR ($ll,X)
            new OpCode { Txt = "eor", Code = 0x49, ArgsNr = 1, Relocate = false }, // EOR #$nn
            new OpCode { Txt = "inc", Code = 0xEE, ArgsNr = 2, Relocate = true }, // INC $hhll
            new OpCode { Txt = "inc", Code = 0xFE, ArgsNr = 2, Relocate = true }, // INC $hhll, X
            new OpCode { Txt = "inc", Code = 0xE6, ArgsNr = 1, Relocate = false }, // INC $ll
            new OpCode { Txt = "inc", Code = 0xF6, ArgsNr = 1, Relocate = false }, // INC $ll, X
            new OpCode { Txt = "inx", Code = 0xE8, ArgsNr = 0, Relocate = false }, // INX
            new OpCode { Txt = "iny", Code = 0xC8, ArgsNr = 0, Relocate = false }, // INY
            new OpCode { Txt = "jmp", Code = 0x4C, ArgsNr = 2, Relocate = true }, // JMP $hhll
            new OpCode { Txt = "jmp", Code = 0x6C, ArgsNr = 2, Relocate = true }, // JMP ($hhll)
            new OpCode { Txt = "jsr", Code = 0x20, ArgsNr = 2, Relocate = true }, // JSR $hhll
            new OpCode { Txt = "lda", Code = 0xAD, ArgsNr = 2, Relocate = true }, // LDA $hhll
            new OpCode { Txt = "lda", Code = 0xBD, ArgsNr = 2, Relocate = true }, // LDA $hhll, X
            new OpCode { Txt = "lda", Code = 0xB9, ArgsNr = 2, Relocate = true }, // LDA $hhll, Y
            new OpCode { Txt = "lda", Code = 0xA5, ArgsNr = 1, Relocate = false }, // LDA $ll
            new OpCode { Txt = "lda", Code = 0xB5, ArgsNr = 1, Relocate = false }, // LDA $ll, X
            new OpCode { Txt = "lda", Code = 0xB1, ArgsNr = 1, Relocate = false }, // LDA ($ll), Y
            new OpCode { Txt = "lda", Code = 0xA1, ArgsNr = 1, Relocate = false }, // LDA ($ll,X)
            new OpCode { Txt = "lda", Code = 0xA9, ArgsNr = 1, Relocate = false }, // LDA #$nn
            new OpCode { Txt = "ldx", Code = 0xAE, ArgsNr = 2, Relocate = true }, // LDX $hhll
            new OpCode { Txt = "ldx", Code = 0xBE, ArgsNr = 2, Relocate = true }, // LDX $hhll, Y
            new OpCode { Txt = "ldx", Code = 0xA6, ArgsNr = 1, Relocate = false }, // LDX $ll
            new OpCode { Txt = "ldx", Code = 0xB6, ArgsNr = 1, Relocate = false }, // LDX $ll, Y
            new OpCode { Txt = "ldx", Code = 0xA2, ArgsNr = 1, Relocate = false }, // LDX #$nn
            new OpCode { Txt = "ldy", Code = 0xAC, ArgsNr = 2, Relocate = true }, // LDY $hhll
            new OpCode { Txt = "ldy", Code = 0xBC, ArgsNr = 2, Relocate = true }, // LDY $hhll, X
            new OpCode { Txt = "ldy", Code = 0xA4, ArgsNr = 1, Relocate = false }, // LDY $ll
            new OpCode { Txt = "ldy", Code = 0xB4, ArgsNr = 1, Relocate = false }, // LDY $ll, X
            new OpCode { Txt = "ldy", Code = 0xA0, ArgsNr = 1, Relocate = false }, // LDY #$nn
            new OpCode { Txt = "lsr", Code = 0x4A, ArgsNr = 0, Relocate = false }, // LSR
            new OpCode { Txt = "lsr", Code = 0x4E, ArgsNr = 2, Relocate = true }, // LSR $hhll
            new OpCode { Txt = "lsr", Code = 0x5E, ArgsNr = 2, Relocate = true }, // LSR $hhll, X
            new OpCode { Txt = "lsr", Code = 0x46, ArgsNr = 1, Relocate = false }, // LSR $ll
            new OpCode { Txt = "lsr", Code = 0x56, ArgsNr = 1, Relocate = false }, // LSR $ll, X
            new OpCode { Txt = "nop", Code = 0xEA, ArgsNr = 0, Relocate = false }, // NOP
            new OpCode { Txt = "ora", Code = 0x0D, ArgsNr = 2, Relocate = true }, // ORA $hhll
            new OpCode { Txt = "ora", Code = 0x1D, ArgsNr = 2, Relocate = true }, // ORA $hhll, X
            new OpCode { Txt = "ora", Code = 0x19, ArgsNr = 2, Relocate = true }, // ORA $hhll, Y
            new OpCode { Txt = "ora", Code = 0x05, ArgsNr = 1, Relocate = false }, // ORA $ll
            new OpCode { Txt = "ora", Code = 0x15, ArgsNr = 1, Relocate = false }, // ORA $ll, X
            new OpCode { Txt = "ora", Code = 0x11, ArgsNr = 1, Relocate = false }, // ORA ($ll), Y
            new OpCode { Txt = "ora", Code = 0x01, ArgsNr = 1, Relocate = false }, // ORA ($ll,X)
            new OpCode { Txt = "ora", Code = 0x09, ArgsNr = 1, Relocate = false }, // ORA #$nn
            new OpCode { Txt = "pha", Code = 0x48, ArgsNr = 0, Relocate = false }, // PHA
            new OpCode { Txt = "php", Code = 0x08, ArgsNr = 0, Relocate = false }, // PHP
            new OpCode { Txt = "pla", Code = 0x68, ArgsNr = 0, Relocate = false }, // PLA
            new OpCode { Txt = "plp", Code = 0x28, ArgsNr = 0, Relocate = false }, // PLP
            new OpCode { Txt = "rol", Code = 0x2A, ArgsNr = 0, Relocate = false }, // ROL
            new OpCode { Txt = "rol", Code = 0x2E, ArgsNr = 2, Relocate = true }, // ROL $hhll
            new OpCode { Txt = "rol", Code = 0x3E, ArgsNr = 2, Relocate = true }, // ROL $hhll, X
            new OpCode { Txt = "rol", Code = 0x26, ArgsNr = 1, Relocate = false }, // ROL $ll
            new OpCode { Txt = "rol", Code = 0x36, ArgsNr = 1, Relocate = false }, // ROL $ll, X
            new OpCode { Txt = "ror", Code = 0x6A, ArgsNr = 0, Relocate = false }, // ROR
            new OpCode { Txt = "ror", Code = 0x6E, ArgsNr = 2, Relocate = true }, // ROR $hhll
            new OpCode { Txt = "ror", Code = 0x7E, ArgsNr = 2, Relocate = true }, // ROR $hhll, X
            new OpCode { Txt = "ror", Code = 0x66, ArgsNr = 1, Relocate = false }, // ROR $ll
            new OpCode { Txt = "ror", Code = 0x76, ArgsNr = 1, Relocate = false }, // ROR $ll, X
            new OpCode { Txt = "rti", Code = 0x40, ArgsNr = 0, Relocate = false }, // RTI
            new OpCode { Txt = "rts", Code = 0x60, ArgsNr = 0, Relocate = false }, // RTS
            new OpCode { Txt = "sbc", Code = 0xED, ArgsNr = 2, Relocate = true }, // SBC $hhll
            new OpCode { Txt = "sbc", Code = 0xFD, ArgsNr = 2, Relocate = true }, // SBC $hhll, X
            new OpCode { Txt = "sbc", Code = 0xF9, ArgsNr = 2, Relocate = true }, // SBC $hhll, Y
            new OpCode { Txt = "sbc", Code = 0xE5, ArgsNr = 1, Relocate = false }, // SBC $ll
            new OpCode { Txt = "sbc", Code = 0xF5, ArgsNr = 1, Relocate = false }, // SBC $ll, X
            new OpCode { Txt = "sbc", Code = 0xF1, ArgsNr = 1, Relocate = false }, // SBC ($ll), Y
            new OpCode { Txt = "sbc", Code = 0xE1, ArgsNr = 1, Relocate = false }, // SBC ($ll,X)
            new OpCode { Txt = "sbc", Code = 0xE9, ArgsNr = 1, Relocate = false }, // SBC #$nn
            new OpCode { Txt = "sec", Code = 0x38, ArgsNr = 0, Relocate = false }, // SEC
            new OpCode { Txt = "sed", Code = 0xF8, ArgsNr = 0, Relocate = false }, // SED
            new OpCode { Txt = "sei", Code = 0x78, ArgsNr = 0, Relocate = false }, // SEI
            new OpCode { Txt = "sta", Code = 0x8D, ArgsNr = 2, Relocate = true }, // STA $hhll
            new OpCode { Txt = "sta", Code = 0x9D, ArgsNr = 2, Relocate = true }, // STA $hhll, X
            new OpCode { Txt = "sta", Code = 0x99, ArgsNr = 2, Relocate = true }, // STA $hhll, Y
            new OpCode { Txt = "sta", Code = 0x85, ArgsNr = 1, Relocate = false }, // STA $ll
            new OpCode { Txt = "sta", Code = 0x95, ArgsNr = 1, Relocate = false }, // STA $ll, X
            new OpCode { Txt = "sta", Code = 0x91, ArgsNr = 1, Relocate = false }, // STA ($ll), Y
            new OpCode { Txt = "sta", Code = 0x81, ArgsNr = 1, Relocate = false }, // STA ($ll,X)
            new OpCode { Txt = "stx", Code = 0x8E, ArgsNr = 2, Relocate = true }, // STX $hhll
            new OpCode { Txt = "stx", Code = 0x86, ArgsNr = 1, Relocate = false }, // STX $ll
            new OpCode { Txt = "stx", Code = 0x96, ArgsNr = 1, Relocate = false }, // STX $ll, Y
            new OpCode { Txt = "sty", Code = 0x8C, ArgsNr = 2, Relocate = true }, // STY $hhll
            new OpCode { Txt = "sty", Code = 0x84, ArgsNr = 1, Relocate = false }, // STY $ll
            new OpCode { Txt = "sty", Code = 0x94, ArgsNr = 1, Relocate = false }, // STY $ll, X
            new OpCode { Txt = "tax", Code = 0xAA, ArgsNr = 0, Relocate = false }, // TAX
            new OpCode { Txt = "tay", Code = 0xA8, ArgsNr = 0, Relocate = false }, // TAY
            new OpCode { Txt = "tsx", Code = 0xBA, ArgsNr = 0, Relocate = false }, // TSX
            new OpCode { Txt = "txa", Code = 0x8A, ArgsNr = 0, Relocate = false }, // TXA
            new OpCode { Txt = "txs", Code = 0x9A, ArgsNr = 0, Relocate = false }, // TXS
            new OpCode { Txt = "tya", Code = 0x98, ArgsNr = 0, Relocate = false }, // TYA

            new OpCode { Txt = "nop", Code = 0x5A, ArgsNr = 0, Relocate = false }, // TYA

            // Illegal opcodes
            new OpCode { Txt = "slo", Code = 0x07, ArgsNr = 1, Relocate = false }, // SLO zp
            new OpCode { Txt = "slo", Code = 0x17, ArgsNr = 1, Relocate = false }, // SLO zp,x
            new OpCode { Txt = "slo", Code = 0x03, ArgsNr = 1, Relocate = false }, // SLO ($ll,X)
            new OpCode { Txt = "slo", Code = 0x13, ArgsNr = 1, Relocate = false }, // SLO ($ll), Y
            new OpCode { Txt = "slo", Code = 0x0f, ArgsNr = 2, Relocate = true }, // SLO $hhll
            new OpCode { Txt = "slo", Code = 0x1f, ArgsNr = 2, Relocate = true }, // SLO $hhll, X
            new OpCode { Txt = "slo", Code = 0x1b, ArgsNr = 2, Relocate = true }, // SLO $hhll, Y

            new OpCode { Txt = "rla", Code = 0x27, ArgsNr = 1, Relocate = false }, // RLA zp
            new OpCode { Txt = "rla", Code = 0x37, ArgsNr = 1, Relocate = false }, // RLA zp,x
            new OpCode { Txt = "rla", Code = 0x23, ArgsNr = 1, Relocate = false }, // RLA ($ll,X)
            new OpCode { Txt = "rla", Code = 0x33, ArgsNr = 1, Relocate = false }, // RLA ($ll), Y
            new OpCode { Txt = "rla", Code = 0x2f, ArgsNr = 2, Relocate = false }, // RLA $hhll
            new OpCode { Txt = "rla", Code = 0x3f, ArgsNr = 2, Relocate = false }, // RLA $hhll, X
            new OpCode { Txt = "rla", Code = 0x3b, ArgsNr = 2, Relocate = false }, // RLA $hhll, Y

            new OpCode { Txt = "sre", Code = 0x47, ArgsNr = 1, Relocate = false }, // SRE zp
            new OpCode { Txt = "sre", Code = 0x57, ArgsNr = 1, Relocate = false }, // SRE zp,x
            new OpCode { Txt = "sre", Code = 0x43, ArgsNr = 1, Relocate = false }, // SRE ($ll,X)
            new OpCode { Txt = "sre", Code = 0x53, ArgsNr = 1, Relocate = false }, // SRE ($ll), Y
            new OpCode { Txt = "sre", Code = 0x4f, ArgsNr = 2, Relocate = true }, // SRE $hhll
            new OpCode { Txt = "sre", Code = 0x5f, ArgsNr = 2, Relocate = true }, // SRE $hhll, X
            new OpCode { Txt = "sre", Code = 0x5b, ArgsNr = 2, Relocate = true }, // SRE $hhll, Y

            new OpCode { Txt = "rra", Code = 0x67, ArgsNr = 1, Relocate = false }, // RRA zp
            new OpCode { Txt = "rra", Code = 0x77, ArgsNr = 1, Relocate = false }, // RRA zp,x
            new OpCode { Txt = "rra", Code = 0x63, ArgsNr = 1, Relocate = false }, // RRA ($ll,X)
            new OpCode { Txt = "rra", Code = 0x73, ArgsNr = 1, Relocate = false }, // RRA ($ll), Y
            new OpCode { Txt = "rra", Code = 0x6f, ArgsNr = 2, Relocate = false }, // RRA $hhll
            new OpCode { Txt = "rra", Code = 0x7f, ArgsNr = 2, Relocate = false }, // RRA $hhll, X
            new OpCode { Txt = "rra", Code = 0x7b, ArgsNr = 2, Relocate = false }, // RRA $hhll, Y

            new OpCode { Txt = "sax", Code = 0x87, ArgsNr = 1, Relocate = false }, // SAX zp
            new OpCode { Txt = "sax", Code = 0x97, ArgsNr = 1, Relocate = false }, // SAX $ll, Y
            new OpCode { Txt = "sax", Code = 0x83, ArgsNr = 1, Relocate = false }, // SAX ($ll,X)
            new OpCode { Txt = "sax", Code = 0x8f, ArgsNr = 2, Relocate = true }, // SAX $hhll

            new OpCode { Txt = "lax", Code = 0xa7, ArgsNr = 1, Relocate = false }, // LAX zp
            new OpCode { Txt = "lax", Code = 0xb7, ArgsNr = 1, Relocate = false }, // LAX $ll, Y
            new OpCode { Txt = "lax", Code = 0xa3, ArgsNr = 1, Relocate = false }, // LAX ($ll,X)
            new OpCode { Txt = "lax", Code = 0xb3, ArgsNr = 1, Relocate = false }, // LAX ($ll), Y
            new OpCode { Txt = "lax", Code = 0xaf, ArgsNr = 2, Relocate = true }, // LAX $hhll
            new OpCode { Txt = "lax", Code = 0xbf, ArgsNr = 2, Relocate = true }, // LAX $hhll, Y

            new OpCode { Txt = "dcp", Code = 0xc7, ArgsNr = 1, Relocate = false }, // DCP zp
            new OpCode { Txt = "dcp", Code = 0xd7, ArgsNr = 1, Relocate = false }, // DCP zp,x
            new OpCode { Txt = "dcp", Code = 0xc3, ArgsNr = 1, Relocate = false }, // DCP ($ll,X)
            new OpCode { Txt = "dcp", Code = 0xd3, ArgsNr = 1, Relocate = false }, // DCP ($ll), Y
            new OpCode { Txt = "dcp", Code = 0xcf, ArgsNr = 2, Relocate = true }, // DCP $hhll
            new OpCode { Txt = "dcp", Code = 0xdf, ArgsNr = 2, Relocate = true }, // DCP $hhll, X
            new OpCode { Txt = "dcp", Code = 0xdb, ArgsNr = 2, Relocate = true }, // DCP $hhll, Y

            new OpCode { Txt = "isc", Code = 0xe7, ArgsNr = 1, Relocate = false }, // ISC zp
            new OpCode { Txt = "isc", Code = 0xf7, ArgsNr = 1, Relocate = false }, // ISC zp,x
            new OpCode { Txt = "isc", Code = 0xe3, ArgsNr = 1, Relocate = false }, // ISC ($ll,X)
            new OpCode { Txt = "isc", Code = 0xf3, ArgsNr = 1, Relocate = false }, // ISC ($ll), Y
            new OpCode { Txt = "isc", Code = 0xef, ArgsNr = 2, Relocate = true }, // ISC $hhll
            new OpCode { Txt = "isc", Code = 0xff, ArgsNr = 2, Relocate = true }, // ISC $hhll, X
            new OpCode { Txt = "isc", Code = 0xfb, ArgsNr = 2, Relocate = true }, // ISC $hhll, Y

            new OpCode { Txt = "anc", Code = 0x0b, ArgsNr = 1, Relocate = false }, // anc #$nn
            new OpCode { Txt = "anc", Code = 0x2b, ArgsNr = 1, Relocate = false }, // anc #$nn

            new OpCode { Txt = "alr", Code = 0x4b, ArgsNr = 1, Relocate = false }, // alr #$nn
            new OpCode { Txt = "arr", Code = 0x6b, ArgsNr = 1, Relocate = false }, // arr #$nn

            new OpCode { Txt = "sbx", Code = 0xcb, ArgsNr = 1, Relocate = false }, // sbx #$nn
            new OpCode { Txt = "noop", Code = 0x04, ArgsNr = 1, Relocate = false }, // noop $nn
            new OpCode { Txt = "jam", Code = 0x02, ArgsNr = 0, Relocate = false } // jam
        };

        // ========================================================================================
        private static void Relocate(int iStart, int iEnd) {
            int i = iStart;
            while (i < iEnd) {
                byte val = _file[i];

                int j = 0;
                while (val != Opcodes[j].Code && j < Opcodes.Length - 1) {
                    j++;
                }

                if (Opcodes[j].Relocate == true && (_file[i + 2] >= ((_pc & 0xff00) >> 8)) && (_file[i + 2] <= (((_pc + _file.Length) & 0xff00) >> 8))) {
                    Console.Write(">{0:X4}: " + Opcodes[j].Txt + " ${1:X4}", (_pc + i - 2), _file[i + 2] * 256 + _file[i + 1]);

                    _file[i + 2] += (byte)_diffRelocate;
                    Console.WriteLine("  ->  " + Opcodes[j].Txt + " ${0:X4}", (_file[i + 2] * 256 + _file[i + 1]));

                    i = i + 1 + Opcodes[j].ArgsNr;
                }
                else {
                    if (j == Opcodes.Length - 1)
                        i += 1;
                    else
                        i = i + Opcodes[j].ArgsNr + 1;
                }
            }
        }

        // ========================================================================================
        private static void Main(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("-        Welcome to defMONRelocator by Samar @2024           -");
                Console.WriteLine("- Parameters:                                                -");
                Console.WriteLine("- (1) input PRG filename                                     -");
                Console.WriteLine("- (2) output PRG filename                                    -");
                Console.WriteLine("- (3) destination address (hex)                              -");
                Console.WriteLine("- (4) new address for FB (hex)                               -");
                Console.WriteLine("- (5) new address for FC (hex)                               -");
                Console.WriteLine("- (6) new address for 96 (hex)                               -");
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("- Example usage:                                             -");
                Console.WriteLine("- defmonrelocator.exe music.prg music-rel.prg 9000 02 03 04  -");
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("- HINT: Relocation aligned to page only allowed.             -");
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine();
            }
            else {
                try {
                    string temp = args[2];

                    string filename = args[0];
                    _addDest = Convert.ToInt16(args[2], 16);

                    Console.WriteLine("Reading '" + filename + "' ...");
                    _file = System.IO.File.ReadAllBytes(filename);

                    _pc = _file[1] * 256 + _file[0];
                    _diffRelocate = ((_addDest - _pc) >> 8) & 0xff;

                    Console.WriteLine("Load address = ${0:X}", _file[1] * 256 + _file[0]);
                    Console.WriteLine("Difference in load address higher byte = ${0:X2}", _diffRelocate);
                    Console.WriteLine("Relocating player code...");

                    Relocate(2 + 0x0000, 2 + 0x031c);
                    Relocate(2 + 0x0321, 2 + 0x0577);
                    Relocate(2 + 0x06b0, 2 + 0x07a7);

                    for (int i = 0x902; i < 0x0a02; i++) {
                        if (_file[i] > 0)
                            _file[i] += (byte)_diffRelocate;
                    }

                    for (int i = 0xa82; i < 0x0b02; i++) {
                        if (_file[i] > 0)
                            _file[i] += (byte)_diffRelocate;
                    }

                    if (args[3] != null && args[4] != null && args[5] != null) {
                        Console.WriteLine("Relocating zeropage usage...", _addFb);

                        _addFb = Convert.ToByte(args[3], 16);
                        _addFc = Convert.ToByte(args[4], 16);
                        _add96 = Convert.ToByte(args[5], 16);

                        for (int i = 2; i < _file.Length; i++) {
                            for (int j = 0; j < TabFb.Length; j++) {
                                if (i == (TabFb[j] - _pc + 2))
                                    _file[i] = _addFb;
                            }

                            for (int j = 0; j < TabFc.Length; j++) {
                                if (i == (TabFc[j] - _pc + 2))
                                    _file[i] = _addFc;
                            }

                            for (int j = 0; j < Tab96.Length; j++) {
                                if (i == (Tab96[j] - _pc + 2))
                                    _file[i] = _add96;
                            }
                        }

                        Console.WriteLine("Changed $FB to ${0:X2}", _addFb);
                        Console.WriteLine("Changed $FC to ${0:X2}", _addFc);
                        Console.WriteLine("Changed $96 to ${0:X2}", _add96);
                    }

                    _file[1] = (byte)((_addDest >> 8) & 0xff);
                    Console.WriteLine("New load address = ${0:X4}", _file[1] * 256 + _file[0]);

                    filename = args[1];
                    Console.WriteLine("Writing '" + filename + "' ...");
                    System.IO.File.WriteAllBytes(filename, _file);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}