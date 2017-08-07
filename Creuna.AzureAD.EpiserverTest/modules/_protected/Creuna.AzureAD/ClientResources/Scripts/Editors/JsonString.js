/*
Dojo widget for editing an untyped json. 
*/

define([
    "dojo/_base/array",
    "dojo/_base/connect",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",

    "dijit/form/Textarea",

    "epi/epi",
    "epi/shell/widget/_ValueRequiredMixin"
],
function (
    array,
    connect,
    declare,
    lang,

    _CssStateMixin,
    _Widget,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,

    Textarea,
    epi,
    _ValueRequiredMixin
) {

    return declare("creunaAzureAD.editors.JsonString", [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin], {

        baseClass: "epiStringList",

        templateString: '<div class="dijitInline" tabindex="-1" role="presentation">\
                            <div data-dojo-attach-point="stateNode, tooltipNode">\
                                <div data-dojo-attach-point="textArea" data-dojo-type="dijit.form.Textarea"></div>\
                            </div>\
                            <br />\
                            <span>${helptext}</span>\
                        </div>',

        intermediateChanges: false,

        value: null,

        multiple: true,

        onChange: function (value) {
            // Event
        },

        postCreate: function () {
            // call base implementation
            this.inherited(arguments);

            // Init textarea and bind event
            this.textArea.set("intermediateChanges", this.intermediateChanges);
            this.connect(this.textArea, "onChange", this._onTextAreaChanged);
        },

        isValid: function () {
            // summary:
            //    Check if widget's value is valid.
            // tags:
            //    protected, override

            return !this.required || this.value;
        },

        _serialize: function (valueObject) {
            if (!valueObject)
                return "";
            return JSON.stringify(valueObject, null, 4);
        },

        _deserialize: function (str) {
            if (!str)
                return null;
            return JSON.parse(str);
        },

        // Setter for value property
        _setValueAttr: function (value) {
            this._setValue(value, true);
        },

        _getValueAttr: function () {
            // summary:
            //   Returns the textbox value as array.
            // tags:
            //    protected, override

            var val = this.textArea && this.textArea.get("value");
            return this._deserialize(val);
        },

        _setReadOnlyAttr: function (value) {
            this._set("readOnly", value);
            this.textArea.set("readOnly", value);
        },

        // Setter for intermediateChanges
        _setIntermediateChangesAttr: function (value) {
            this.textArea.set("intermediateChanges", value);
            this._set("intermediateChanges", value);
        },

        // Event handler for textarea
        _onTextAreaChanged: function (value) {
            this._setValue(value, false);
        },

        _setValue: function (value, updateTextarea) {
            // Assume value is an array
            var valueObject = value;

            if (typeof value === "string") {
                // Split list
                valueObject = this._deserialize(value);

            } else if (!value) {
                valueObject = {};
            }

            if (this._started && epi.areEqual(this.value, valueObject)) {
                return;
            }

            // set value to this widget (and notify observers)
            this._set("value", valueObject);

            // set value to textarea
            updateTextarea && this.textArea.set("value", this._serialize(valueObject));

            if (this._started && this.validate()) {
                // Trigger change event
                this.onChange(valueObject);
            }
        }
    });
});
