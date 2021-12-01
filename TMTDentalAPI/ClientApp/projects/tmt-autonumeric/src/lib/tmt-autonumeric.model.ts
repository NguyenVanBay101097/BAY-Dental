// tslint:disable-next-line:max-line-length
export interface AutonumericOptions {
    /* Defines if the decimal places should be padded with zeroes
       * `true`     : always pad decimals with zeros (ie. '12.3400')
       * `false`    : never pad with zeros (ie. '12.34')
       * `'floats'` : pad with zeroes only when there are decimals (ie. '12' and '12.3400')
       * Note: setting allowDecimalPadding to 'false' will override the 'decimalPlaces' setting.
       */
    allowDecimalPadding?: boolean | string;

    /* Defines if the decimal character or decimal character alternative should be accepted when there is already
     * a decimal character shown in the element.
     * If set to `true`| any decimal character input will be accepted and will subsequently modify the decimal character position|
     * as well as the `rawValue`.
     * If set to `false`| the decimal character and its alternative key will be dropped as before. This is the default setting.
     */
    alwaysAllowDecimalCharacter?: boolean;

    /* Defines where should be positioned the caret on focus
     * null : Do not enforce any caret positioning on focus (this is needed when using `selectOnFocus`)
     * `'start'` : put the caret of the far left side of the value (excluding the positive/negative sign and currency symbol| if any)
     * `'end'` : put the caret of the far right side of the value (excluding the positive/negative sign and currency symbol| if any)
     * `'decimalLeft'` : put the caret of the left of the decimal character if any
     * `'decimalRight'` : put the caret of the right of the decimal character if any
     */
    caretPositionOnFocus?: string | null;

    /* Defines if a local list of AutoNumeric objects should be kept when initializing this object.
     * This list is used by the `global.*` functions.
     */
    createLocalList?: boolean;

    /* Defines the currency symbol string.
     * It can be a string of more than one character (allowing for instance to use a space on either side of it| example: '$ ' or ' $')
     * cf. https://en.wikipedia.org/wiki/Currency_symbol
     */
    currencySymbol?: string;


    /* Defines where the currency symbol should be placed (before of after the numbers)
     * for prefix currencySymbolPlacement: "p" (default)
     * for suffix currencySymbolPlacement: "s"
     */
    currencySymbolPlacement?: string;

    /* Defines what decimal separator character is used
     */
    decimalCharacter?: string;

    /* Allow to declare an alternative decimal separator which is automatically replaced by `decimalCharacter` when typed.
     * This is used by countries that use a comma '|' as the decimal character and have keyboards with a numeric pads that have
     * a period 'full stop' as the decimal character (France or Spain for instance).
     */
    decimalCharacterAlternative?: null | string;

    /* Defines the default number of decimal places to show on the formatted value| and keep for the precision.
     * Incidentally| since we need to be able to show that many decimal places| this also defines the raw value precision by default.
     */
    decimalPlaces?: number;
    /* Defines how many decimal places should be kept for the raw value (ie. This is the precision for float values).
     *
     * If this option is set to `null` (which is the default)| then the value of `decimalPlaces` is used for `decimalPlacesRawValue` as well.
     * Note: Setting this to a lower number of decimal places than the one to be shown will lead to confusion for the users.
     */
    decimalPlacesRawValue?: null | number;

    /* Defines how many decimal places should be visible when the element is unfocused.
     * If this is set to `null`| then this option is ignored| and the `decimalPlaces` option value will be used instead.
     * This means this is optional ; if omitted the decimal places will be the same when the input has the focus.
     *
     * This option can be used in conjonction with the two other `scale*` options| which allows to display
      a different formatted value when the element is unfocused| while another formatted value is shown when focused.
     */
    decimalPlacesShownOnBlur?: null | number;

    /* Defines how many decimal places should be visible when the element has the focus.
     * If this is set to `null`| then this option is ignored| and the `decimalPlaces` option value will be used instead.
     *
     * Example:
     * For instance if `decimalPlacesShownOnFocus` is set to `5` and the default number of decimal places is `2`| then on focus `1|000.12345`
     * will be shown| while without focus `1|000.12` will be set back.
     * Note 1: the results depends on the rounding method used.
     * Note 2: the `getNumericString()` method returns the extended decimal places
     */
    decimalPlacesShownOnFocus?: null | number;

    /* Helper option for ASP.NET postback
     * This should be set as the value of the unformatted default value
     * examples:
     * no default value="" {defaultValueOverride: ""}
     * value=1234.56 {defaultValueOverride: '1234.56'}
     */
    defaultValueOverride?: null;

    /* Defines how many numbers should be grouped together (usually for the thousand separator)
     * - "2"|  results in 99|99|99|999 India's lakhs
     * - "2s"| results in 99|999|99|99|999 India's lakhs scaled
     * - "3"|  results in 999|999|999 (default)
     * - "4"|  results in 9999|9999|9999 used in some Asian countries
     * Note: This option does not accept other grouping choice.
     */
    digitalGroupSpacing?: string | number;

    /* Defines the thousand grouping separator character
     * Example : If `'.'` is set| then you'll get `'1.234.567'`
     */
    digitGroupSeparator?: string;

    /* The `divisorWhenUnfocused` divide the element value on focus.
     * On blur| the element value is multiplied back.
     * Example : Display percentages using { divisorWhenUnfocused: 100 } (or directly in the Html with
     *`<input data-divisor-when-unfocused="100">`)
     * The divisor value does not need to be an integer| but please understand that Javascript has limited accuracy in math;
      use with caution.
     * Note: The `getNumericString` method returns the full value| including the 'hidden' decimals.
     */
    divisorWhenUnfocused?: null | number;

    /* Defines what should be displayed in the element if the raw value is an empty string ('').
     * - 'focus'  : The currency sign is displayed when the input receives focus (default)
     * - 'press'  : The currency sign is displayed whenever a key is being pressed
     * - 'always' : The currency sign is always displayed
     * - 'zero'   : A zero is displayed ('rounded' with or without a currency sign) if the input has no value on focus out
     * - 'min'    : The minimum value is displayed if the input has no value on focus out
     * - 'max'    : The maximum value is displayed if the input has no value on focus out
     * - 'null'   : When the element is empty| the `rawValue` and the element value/text is set to `null`. This also allows to set
     *  the value to `null` using `anElement.set(null)`.
     */
    emptyInputBehavior?: string;

    /* Defines if the custom and native events triggered by AutoNumeric should bubble up or not.
     */
    eventBubbles?: boolean;

    /* Defines if the custom and native events triggered by AutoNumeric should be cancelable.
     */
    eventIsCancelable?: boolean;

    /* This option is the 'strict mode' (aka 'debug' mode)| which allows autoNumeric to strictly analyse the options passed|
     * and fails if an unknown options is used in the settings object.
     * You should set that to `true` if you want to make sure you are only using 'pure' autoNumeric settings objects in your code.
     * If you see uncaught errors in the console and your code starts to fail| this means somehow those options gets polluted by
     *  another program (which usually happens when using frameworks).
     */
    failOnUnknownOption?: boolean;

    /* Determine if the default value will be formatted on initialization.
     */
    formatOnPageLoad?: boolean;

    /* Defines if the 'formula mode' can be activated by the user.
     * If set to `true`| then the user can enter the formula mode by entering the '=' character.
     * He will then be allowed to enter any simple math formula using numeric characters as well as
     * the following operators +| -| *| /| ( and ).
     * The formula mode is closed when the user either validate their math expression using the `Enter` key| or when the element is blurred.
     * If the formula is invalid| the previous valid `rawValue` is set back| and the `autoNumeric:invalidFormula` event is sent.
     * When a valid formula is accepted| then its result is `set()`| and the `autoNumeric:validFormula` event is sent.
     *
     * By default| this mode is disabled.
     */
    formulaMode?: boolean;

    /* Set the undo/redo history table size.
     * Each record keeps the raw value as well and the last known caret/selection positions.
     */
    historySize?: number;
    /* Allow the user to 'cancel' and undo the changes he made to the given autonumeric-managed element| by pressing the 'Escape' key.
     * Whenever the user 'validate' the input (either by hitting 'Enter'| or blurring the element)| the new value is saved for subsequent
     *  'cancellation'.
     * The process :
     *   - save the input value on focus
     *   - if the user change the input value| and hit `Escape`| then the initial value saved on focus is set back
     *   - on the other hand if the user either have used `Enter` to validate (`Enter` throws a change event) his entries|
     *  or if the input value has been changed by another script in the mean time| then we save the new input value
     *   - on a successful 'cancel'| select the whole value (while respecting the `selectNumberOnly` option)
     *   - bonus; if the value has not changed| hitting 'Esc' just select all the input value (while respecting the `selectNumberOnly` option)
     */
    isCancellable?: boolean;

    /* Controls the leading zero behavior
     * - 'allow' : allows leading zeros to be entered. Zeros will be truncated when entering additional digits.
     *  On focusout zeros will be deleted
     * - 'deny'  : allows only one leading zero on values that are between 1 and -1
     * - 'keep'  : allows leading zeros to be entered. on focusout zeros will be retained
     */
    leadingZero?: string;

    /* Defines the maximum possible value a user can enter.
     * Notes:
     * - this value must be a string and use the period for the decimal point
     * - this value needs to be larger than `minimumValue`
     */
    maximumValue?: string | number;

    /* Defines the minimum possible value a user can enter.
     * Notes:
     * - this value must be a string and use the period for the decimal point
     * - this value needs to be smaller than `maximumValue`
     * - if this is superior to 0| then you'll effectively prevent your user to entirely delete the content of your element
     */
    minimumValue?: string | number;

    /* Allows the user to increment or decrement the element value with the mouse wheel.
     * The wheel behavior can be modified by the `wheelStep` option.
     * This `wheelStep` option can be used in two ways| either by setting:
     * - a 'fixed' step value (`wheelStep : 1000`)| or
     * - the 'progressive' string (`wheelStep : 'progressive'`)| which will then activate a special mode where the step is automatically
     *  calculated based on the element value size.
     *
     * Note :
     * You can activate/deactivate the wheel event for each `wheelOn` option value by using the 'Shift' modifier key while using
     *  the mouse wheel.
     */
    modifyValueOnWheel?: boolean;

    /* Adds brackets on negative values (ie. transforms '-$ 999.99' to '($999.99)')
     * Those brackets are visible only when the field does NOT have the focus.
     * The left and right symbols should be enclosed in quotes and separated by a comma.
     */
    negativeBracketsTypeOnBlur?: string | null;

    /* Placement of the negative/positive sign relative to the `currencySymbol` option.
     *
     * Example:
     * // Default values
     * -1|234.56  => default no options required
     * $-1|234.56 => {currencySymbol: "$"| negativePositiveSignPlacement: "r"} // Default if negativePositiveSignPlacement is
     *  'null' and currencySymbol is not empty
     *
     * // Sign on the left hand side of the whole number
     * -$1|234.56 => {currencySymbol: "$"} or {currencySymbol: "$"| negativePositiveSignPlacement: "l"}
     * -1|234.56$ => {currencySymbol: "$"| currencySymbolPlacement: "s"| negativePositiveSignPlacement: "p"}
     * // Default if negativePositiveSignPlacement is 'null' and currencySymbol is not empty
     *
     * // Sign on the right hand side of the whole number
     * 1|234.56-  => {negativePositiveSignPlacement: "s"}
     * $1|234.56- => {currencySymbol: "$"| negativePositiveSignPlacement: "s"}
     * 1|234.56-$ => {currencySymbol: "$"| currencySymbolPlacement: "s"}
     * 1|234.56$- => {currencySymbol: "$"| currencySymbolPlacement: "s"| negativePositiveSignPlacement: "r"}
     */
    negativePositiveSignPlacement?: string | null;

    /* Defines the negative sign symbol.
     * It can be a string of only one character.
     */
    negativeSignCharacter?: string;

    /* Defines if the element should have event listeners activated on it.
     * By default| those event listeners are only added to <input> elements and html element with the `contenteditable`
     * attribute set to `true`| but not on the other html tags.
     * This allows to initialize elements without any event listeners.
     * Warning: Since AutoNumeric will not check the input content after its initialization| using some autoNumeric
     * methods afterwards *will* probably leads to formatting problems.
     */
    noEventListeners?: boolean;

    /* Manage how autoNumeric react when the user tries to paste an invalid number.
     * - 'error'    : (This is the default behavior) The input value is not changed and an error is output in the console.
     * - 'ignore'   : idem than 'error'| but fail silently without outputting any error/warning in the console.
     * - 'clamp'    : if the pasted value is either too small or too big regarding the minimumValue and maximumValue range|
     * then the result is clamped to those limits.
     * - 'truncate' : autoNumeric will insert as many pasted numbers it can at the initial caret/selection| until everything is pasted|
     * or the range limit is hit.
     *                The non-pasted numbers are dropped and therefore not used at all.
     * - 'replace'  : autoNumeric will first insert as many pasted numbers it can at the initial caret/selection| then if the range limit
     * is hit| it will try
     *                to replace one by one the remaining initial numbers (on the right side of the caret) with the rest of the pasted
     *  numbers.
     *
     * Note 1 : A paste content starting with a negative sign '-' will be accepted anywhere in the input| and will set the resulting value
     *  as a negative number
     * Note 2 : A paste content starting with a number will be accepted| even if the rest is gibberish (ie. '123foobar456').
     *          Only the first number will be used (here '123').
     * Note 3 : The paste event works with the `decimalPlacesShownOnFocus` option too.
     */
    onInvalidPaste?: string;

    /* Defines how the value should be formatted when wanting a 'localized' version of it.
     * - null or 'string' => 'nnnn.nn' or '-nnnn.nn' as text type. This is the default behavior.
     * - 'number'         => nnnn.nn or -nnnn.nn as a Number (Warning: this works only for integers inferior to Number.MAX_SAFE_INTEGER)
     * - '|' or '-|'      => 'nnnn|nn' or '-nnnn|nn'
     * - '.-'             => 'nnnn.nn' or 'nnnn.nn-'
     * - '|-'             => 'nnnn|nn' or 'nnnn|nn-'
     *
     * Note: The hyphen '-' is translated to the custom negative sign defined in `negativeSignCharacter`
     */
    outputFormat?: string | null;
    /* Override the minimum and maximum limits
     * overrideMinMaxLimits: "ceiling" adheres to maximumValue and ignores minimumValue settings
     * overrideMinMaxLimits: "floor" adheres to minimumValue and ignores maximumValue settings
     * overrideMinMaxLimits: "ignore" ignores both minimumValue & maximumValue
     */
    overrideMinMaxLimits?: string | null;
    /* Defines the positive sign symbol.
     * It can be a string of only one character.
     * This is shown only if `showPositiveSign` is set to `true`.
     */
    positiveSignCharacter?: string | null;

    /* The `rawValueDivisor` divides the formatted value shown in the AutoNumeric element and store the result in `rawValue`.
     * @example { rawValueDivisor: '100' } or <input data-raw-value-divisor="100">
     * Given the `0.01234` raw value| the formatted value will be displayed as `'1.234'`.
     * This is useful when displaying percentage for instance| and avoid the need to divide/multiply by 100
     * between the number shown and the raw value.
     */
    rawValueDivisor?: null | number;

    /* Defines if the element (`<input>` or another allowed html tag) should be set as read-only on initialization.
     * When set to `true`| then:
     * - the `readonly` html property is added to the <input> element on initialization| or
     * - the `contenteditable` attribute is set to `false` on non-input elements.
     */
    readOnly?: boolean;

    /* Defines the rounding method to use.
     * roundingMethod: "S"| Round-Half-Up Symmetric (default)
     * roundingMethod: "A"| Round-Half-Up Asymmetric
     * roundingMethod: "s"| Round-Half-Down Symmetric (lower case s)
     * roundingMethod: "a"| Round-Half-Down Asymmetric (lower case a)
     * roundingMethod: "B"| Round-Half-Even "Bankers Rounding"
     * roundingMethod: "U"| Round Up "Round-Away-From-Zero"
     * roundingMethod: "D"| Round Down "Round-Toward-Zero" - same as truncate
     * roundingMethod: "C"| Round to Ceiling "Toward Positive Infinity"
     * roundingMethod: "F"| Round to Floor "Toward Negative Infinity"
     * roundingMethod: "N05" Rounds to the nearest .05 => same as "CHF" used in 1.9X and still valid
     * roundingMethod: "U05" Rounds up to next .05
     * roundingMethod: "D05" Rounds down to next .05
     */
    roundingMethod?: string;
    /* Set to `true` to allow the `decimalPlacesShownOnFocus` value to be saved with sessionStorage
     * If IE 6 or 7 is detected| the value will be saved as a session cookie.
     */
    saveValueToSessionStorage?: boolean;

    /* Determine if the select all keyboard command will select the complete input text| or only the input numeric value
     * Note : If the currency symbol is between the numeric value and the negative sign| only the numeric value will be selected
     */
    selectNumberOnly?: boolean;

    /* Defines if the element value should be selected on focus.
     * Note: The selection is done using the `selectNumberOnly` option.
     */
    selectOnFocus?: boolean;

    /* Defines how the serialize functions should treat the spaces.
     * Those spaces ' ' can either be converted to the plus sign '+'| which is the default| or to '%20'.
     * Both values being valid per the spec (http://www.w3.org/Addressing/URL/uri-spec.html).
     * Also see the summed up answer on http://stackoverflow.com/a/33939287.
     *
     * tl;dr : Spaces should be converted to '%20' before the '?' sign| then converted to '+' after.
     * In our case since we serialize the query| we use '+' as the default (but allow the user to get back the old *wrong* behavior).
     */
    serializeSpaces?: string;
    /* Defines if the element value should be converted to the raw value on focus (and back to the formatted on blur).
     * If set to `true`| then autoNumeric remove the thousand separator| currency symbol and suffix on focus.
     * Example:
     * If the input value is '$ 1|999.88 suffix'| on focus it becomes '1999.88' and back to '$ 1|999.88 suffix' on blur.
     */
    showOnlyNumbersOnFocus?: boolean;

    /* Allow the positive sign symbol `+` to be displayed for positive numbers.
     * By default| this positive sign is not shown.
     * The sign placement is controlled by the 'negativePositiveSignPlacement' option| mimicking the negative sign placement rules.
     */
    showPositiveSign?: boolean;

    /* Defines if warnings should be shown in the console.
     * Those warnings can be ignored| but are usually printed when something could be improved by the user (ie. option conflicts).
     */
    showWarnings?: boolean;

    /* Defines the rules that calculate the CSS class(es) to apply on the element| based on the raw unformatted value.
     * This can also be used to call callbacks whenever the `rawValue` is updated.
     * Important: all callbacks must return `null` if no ranges/userDefined classes are selected
     * @example
     * {
     *     positive   : 'autoNumeric-positive'| // Or `null` to not use it
     *     negative   : 'autoNumeric-negative'|
     *     ranges     : [
     *         { min: 0| max: 25| class: 'autoNumeric-red' }|
     *         { min: 25| max: 50| class: 'autoNumeric-orange' }|
     *         { min: 50| max: 75| class: 'autoNumeric-yellow' }|
     *         { min: 75| max: Number.MAX_SAFE_INTEGER| class: 'autoNumeric-green' }|
     *     ]|
     *     userDefined: [
     *         // If 'classes' is a string| set it if `true`| remove it if `false`
     *         { callback: rawValue => { return true; }| classes: 'thisIsTrue' }|
     *         // If 'classes' is an array with only 2 elements| set the first class if `true`| the second if `false`
     *         { callback: rawValue => rawValue % 2 === 0| classes: ['autoNumeric-even'| 'autoNumeric-odd'] }|
     *         // Return only one index to use on the `classes` array (here| 'class3')
     *         { callback: rawValue => { return 2; }| classes: ['class1'| 'class2'| 'class3'] }|
     *         // Return an array of indexes to use on the `classes` array (here| 'class1' and 'class3')
     *         { callback: rawValue => { return [0| 2]; }| classes: ['class1'| 'class2'| 'class3'] }|
     *         // If 'classes' is `undefined` or `null`| then the callback is called with the AutoNumeric object passed as a parameter
     *         { callback: anElement => { return anElement.getFormatted(); } }|
     *     ]|
     * }
     */
    styleRules?: any;
    /* Add a text on the right hand side of the element value.
     * This suffix text can have any characters in its string| except numeric characters and the negative/positive sign.
     * Example: ' dollars'
     */
    suffixText?: string;

    /* The three options (divisorWhenUnfocused| decimalPlacesShownOnBlur & symbolWhenUnfocused) handle scaling
     * of the input when the input does not have focus
     * Please note that the non-scaled value is held in data and it is advised that you use the `saveValueToSessionStorage`
     *  option to ensure retaining the value
     * ["divisor"| "decimal places"| "symbol"]
     * Example: with the following options set {divisorWhenUnfocused: '1000'| decimalPlacesShownOnBlur: '1'| symbolWhenUnfocused: ' K'}
     * Example: focusin value "1|111.11" focusout value "1.1 K"
     */

    /* The `symbolWhenUnfocused` option is a symbol placed as a suffix when not in focus.
     * This is optional too.
     */
    symbolWhenUnfocused?: null | string;

    /* Defines if the element value should be unformatted when the user hover his mouse over it while holding the `Alt` key.
     * Unformatting there means that this removes any non-number characters and displays the *raw* value| as understood
     * by Javascript (ie. `12.34` is a valid number| while `12|34` is not).
     * We reformat back before anything else if :
     * - the user focus on the element by tabbing or clicking into it|
     * - the user releases the `Alt` key| and
     * - if we detect a mouseleave event.
     *
     * We unformat again if :
     * - while the mouse is over the element| the user hit `Alt` again
     */
    unformatOnHover?: boolean;

    /* Removes the formatting and use the raw value in each autoNumeric elements of the parent form element| on the form `submit` event.
     * The output format is a numeric string (nnnn.nn or -nnnn.nn).
     */
    unformatOnSubmit?: boolean;

    /* Provides a way for automatically replacing the formatted value with a pre-defined string| when the raw value is equal to a
     * specific value
     * Here you can specify as many 'conversion' as needed.
     */
    valuesToStrings?: null | { 0: '-', } | { '-1': 'Min', 1: 'Max', };
    /* Defines if the AutoNumeric element should watch external changes made without using `.set()`| but by using the basic
     * `aNElement.node().value = 42` notation.
     * If set to `watch`| then AutoNumeric will format the new value using `.set()` internally.
     * Otherwise it will neither format it| nor save it in the history.
     */
    watchExternalChanges?: boolean;

    /* Defines when the wheel event will increment or decrement the element value.
     * When set to `'focus'`| the AutoNumeric-managed element needs to be focused for the wheel event to change the value.
     * When set to `'hover'`| using the wheel event while the mouse is hovering the element is sufficient (no focus needed).
     *
     * Note :
     * When `wheelOn` is set to `'focus'`| you can use the 'Shift' modifier key while using the mouse wheel in order
     * to temporarily activate the increment/decrement feature even if the element is not focused.
     * When `wheelOn` is set to `'hover'`| you can use the 'Shift' modifier key while using the mouse wheel in order
     *  to temporarily disable the increment/decrement feature even if the element is not hovered.
     */
    wheelOn?: string;

    /* That option is linked to the `modifyValueOnWheel` one and will only be used if the latter is set to `true`.
     * This option will modify the wheel behavior and can be used in two ways| either by setting :
     * - a 'fixed' step value (a positive float or integer number `1000`)| or
     * - the `'progressive'` string.
     *
     * The 'fixed' mode always increment/decrement the element value by that amount| while respecting
     *  the `minimumValue` and `maximumValue` settings.
     * The 'progressive' mode will increment/decrement the element value based on its current value. The bigger the
     * number| the bigger the step| and vice versa.
     */
    wheelStep?: string;

    /* Options Change strategy,
     *
     */
}
