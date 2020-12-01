# ScribeLang Documentation
An experimental, work in progress scripting language.

## Syntax

### Data Types, Declaration, and Assignment

| Data Type          | Token               | Size                        | Evaluation |
| ------------------ | ------------------- | --------------------------- | ---------- |
| explicitly empty   | `nul`               | 0                           | value      |
| any (determined)   | `var`               | variable                    | any        |
| boolean            | `boo`               | 1 bit                       | value      |
| integers un/signed | `u[bits]`/`i[bits]` | 2^n, 08 doubling to 64 bits | value      |
| floating point     | `f32/f64`           | 2^32 or 2^64 bits           | value      |
| decimal            | `dec`               | variable                    | value      |
| string             | `str`               | variable                    | value      ||

Assignment can be of any evaluated code. Possible methods of declaration and assignment:

    i32 variable1      //Declared
	variable1 = 42     //Assigned
	i32 variable2 = 42 //Declared & Assigned

### Subroutines

#### Named

| Scheme                    | Syntax                                  |
| ------------------------- | --------------------------------------- |
| No parameters, no returns | `sub func => code`                      |
| No parameters, returns    | `sub func: i32 => value`                |
| Parameters, no returns    | `sub func (i32 a, i32 b) => code`       |
| Parameters, returns       | `sub func (i32 a, i32 b): i32 => value` |

Multi-line:

    def func sum(i32 a, i32 b): i32 
        i32 sum = a + b
        => sum

Evaluate subroutine with arguments: `sum(1, $arg2)`

#### Evaluation routines

    i32 valueA = 23
    i32 valueB = 22
	i32 sum = [[+ $valueA $valueB]] // 45

### Flow Control

| Statement                      | Action                                                                              |
| ------------------------------ | ----------------------------------------------------------------------------------- |
| `if [[condition]]`                 | Evaluate block if condition is true                                                 |
| `else`                         | Evaluate block if previous `if` condition was false                                 |
| `elif [[condition]]`               | Same as `else`, and its condition is true                                           |
| `[[condition]] ? true : false`     | Evaluate blocks depending on condition                                              |
| `when [[condition]]`               | Evaluate block when condition is true, only once                                    |
| `while [[condition]]`              | Loop block if present while condition is true                                       |
| `for [[first; condition; repeat]]` | Evaluate `first` if present, loop block & repeat if present while condition is true |
| `each [[item: array]]`             | Loop block, advance item through array each loop                                    |
| `skip`                         | Skip to next iteration                                                              |
| `finish`                       | Finish loop                                                                         |

#### Conditionals

Conditionals are evaluated as true when their integer representation is non-zero.

### Operators

#### Unary (1 Operand)

| Operator | Action                    |
| -------- | ------------------------- |
| `!a`     | logical negate            |
| `++a`    | pre-evaluation increment  |
| `--a`    | pre-evaluation decrement  |
| `a++`    | post-evaluation increment |
| `a--`    | post-evaluation decrement |
| `a[n]`   | array access element `n`  |

#### Binary (2 Operands)

| Operator                | Action                      |
| ----------------------- | --------------------------- |
| `a = b`                 | variable assignment         |
| `+ a b`                 | arithmetical addition       |
| `- a b`                 | arithmetical subtraction    |
| `* a b`                 | arithmetical multiplication |
| `/ a b`                 | arithmetical division       |
| `% a b`                 | arithmetical modulo         |
| `& a b`                 | bitwise AND                 |
| <code>&#124; a b</code> | bitwise OR                  |
| `^ a b`                 | bitwise XOR                 |
| `>> a b`                | bitwise right shift         |
| `<< a b`                | bitwise left shift          |

| Conditional operator | Action                                          |
| -------------------- | ----------------------------------------------- |
|<code>&#124;&#124; a b</code>| OR|
|`&& a b`| AND|
|`< a b`| Less Than|
|`> a b`| Greater Than|
|`== a b`| Equal To|
|`<= a b`| `a` Less Than or Equal To `b`|
|`>= a b`| `a` Greater Than or Equal To `b`|
|`%% a b`| `a` Contains `b`|
|`*% a b`|`a` Starts with `b`|
|`%* a b` |`a` Ends with `b`|

### Classes / Plugins
Plugins are JSON files which contain references to classes and functions within the host language, and can be mapped to a desired alias. Functions that are mapped can be restricted, or allow all with ease based on the environment or scenario you'd want to run the language.

i.e term-write-only.json
```
{
  "ClassName": "Console",
  "Alias": "term",
  "AssemblyName": "System.Console, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
  "Methods": [
    {
      "MethodName": "WriteLine",
      "Alias": "log",
      "Parameters": [
        {
          "Name": "value",
          "Type": "String"
        }
      ]
    }
  ]
}    
```

Member access is denoted by `->`
i.e `term->log("Hello, world")`

### Class/Plugin Includes

TODO - currently all plugins within `Scribe.Interpreter.Interactive` are enabled by default.

### Error handling

`code1 !! code2`     Evaluate code2 if code1 reaches an erroneous state  