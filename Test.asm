global _start

section .data

	; strings
	string0: db `9223372036854775807 = `
	string0_length: equ $ - string0
	string1: db `\n`
	string1_length: equ $ - string1
	string2: db `-9223372036854775807 = `
	string2_length: equ $ - string2
	
	%define SYSCALL_WRITE 1
	%define SYSCALL_EXIT 60
	%define STDOUT 1

section .text

_start:
	; DefinitionStatement(Identifier=highest, Value=IntegerExpression(9223372036854775807))[1:1]
	; IntegerExpression(9223372036854775807)
	mov rax, 9223372036854775807
	push rax
	; PrintStatement(StringExpression(0))[2:1]
	; print string
	mov rsi, string0
	mov rdx, string0_length
	mov rax, SYSCALL_WRITE
	mov rdi, STDOUT
	syscall
	; PrintStatement(VariableExpression(highest))[3:1]
	; VariableExpression(highest)
	push QWORD[rsp + 0 * 8]
label1_print_signed_integer:
	pop rax
	; capture FLAGS for signed value
	test rax, rax
	pushf
	; to unsigned value
	mov rbx, rax
	neg rax
	cmovl rax, rbx
	; buffer unsigned value to string
	mov rbx, 10
	mov rsi, rsp
.next_digit:
	dec rsi
	xor rdx, rdx
	div rbx
	add dl, '0'
	mov [rsi], BYTE dl
	test rax, rax
	jnz .next_digit
	; check sign of original value
	popf
	jns .is_positive
	; prepend minus sign
	dec rsi
	mov [rsi], BYTE '-'
.is_positive:
	; print buffer
	lea rdx, [rsp - 8]
	sub rdx, rsi
	mov rax, SYSCALL_WRITE
	mov rdi, STDOUT
	syscall
	; PrintStatement(StringExpression(1))[4:1]
	; print string
	mov rsi, string1
	mov rdx, string1_length
	mov rax, SYSCALL_WRITE
	mov rdi, STDOUT
	syscall
	; DefinitionStatement(Identifier=lowest, Value=IntegerExpression(-9223372036854775807))[6:1]
	; IntegerExpression(-9223372036854775807)
	mov rax, -9223372036854775807
	push rax
	; PrintStatement(StringExpression(2))[7:1]
	; print string
	mov rsi, string2
	mov rdx, string2_length
	mov rax, SYSCALL_WRITE
	mov rdi, STDOUT
	syscall
	; PrintStatement(VariableExpression(lowest))[8:1]
	; VariableExpression(lowest)
	push QWORD[rsp + 0 * 8]
label2_print_signed_integer:
	pop rax
	; capture FLAGS for signed value
	test rax, rax
	pushf
	; to unsigned value
	mov rbx, rax
	neg rax
	cmovl rax, rbx
	; buffer unsigned value to string
	mov rbx, 10
	mov rsi, rsp
.next_digit:
	dec rsi
	xor rdx, rdx
	div rbx
	add dl, '0'
	mov [rsi], BYTE dl
	test rax, rax
	jnz .next_digit
	; check sign of original value
	popf
	jns .is_positive
	; prepend minus sign
	dec rsi
	mov [rsi], BYTE '-'
.is_positive:
	; print buffer
	lea rdx, [rsp - 8]
	sub rdx, rsi
	mov rax, SYSCALL_WRITE
	mov rdi, STDOUT
	syscall
	; PrintStatement(StringExpression(1))[9:1]
	; print string
	mov rsi, string1
	mov rdx, string1_length
	mov rax, SYSCALL_WRITE
	mov rdi, STDOUT
	syscall
	; AssertStatement()[11:1]
	; BinaryOperationExpression(Left=VariableExpression(highest), Right=BinaryOperationExpression(Left=VariableExpression(lowest), Right=IntegerExpression(0)))
	; VariableExpression(highest)
	push QWORD[rsp + 1 * 8]
	; BinaryOperationExpression(Left=VariableExpression(lowest), Right=IntegerExpression(0))
	; VariableExpression(lowest)
	push QWORD[rsp + 1 * 8]
	; IntegerExpression(0)
	push 0
	pop rbx
	pop rax
	; ==
	xor rcx, rcx
	cmp rax, rbx
	sete cl
	push rcx
	pop rbx
	pop rax
	; +
	add rax, rbx
	push rax
	; assert
	pop rax
	test rax, rax
	jnz label3_assert_pass
	; exit code 134
	mov rdi, 134
	mov rax, SYSCALL_EXIT
	syscall
label3_assert_pass:
	; default exit with code 0
	mov rdi, 0
	mov rax, SYSCALL_EXIT
	syscall
