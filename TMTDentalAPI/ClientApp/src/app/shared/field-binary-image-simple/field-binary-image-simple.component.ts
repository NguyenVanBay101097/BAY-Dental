import { Component, OnInit, ViewChild, ViewContainerRef, ComponentFactoryResolver, HostListener, Input, Output, EventEmitter, forwardRef, AfterViewInit, ElementRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-field-binary-image-simple',
  templateUrl: './field-binary-image-simple.component.html',
  styleUrls: ['./field-binary-image-simple.component.css'],
  host: {
    class: 'o_field_image o_field_widget oe_avatar'
  },
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FieldBinaryImageSimpleComponent),
      multi: true,
    }
  ]
})
export class FieldBinaryImageSimpleComponent implements AfterViewInit, ControlValueAccessor {
  placeholder = '/assets/images/placeholder.png';
  // placeholder = 'https://i0.wp.com/www.winhelponline.com/blog/wp-content/uploads/2017/12/user.png?fit=256%2C256&quality=100&ssl=1';
  source: string;
  @ViewChild('inputfile', { static: true }) inputfile: ElementRef;
  @Input('imgSource') imgSource;

  constructor() { }

  ngAfterViewInit(): void {
    this.source = this.imgSource ? this.imgSource : this.placeholder;
  }

  writeValue(obj: any): void {
    if (obj) {
      this.source = obj;
    } else {
      this.source = this.placeholder;
    }
  }

  registerOnChange(fn: any): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: any): void {
  }

  setDisabledState?(isDisabled: boolean): void {
  }

  // the method set in registerOnChange to emit changes back to the form
  private propagateChange = (_: any) => { };

  renderImage() {
  }

  onFileChange(e) {
    console.log(e);
    var file_node = e.target;
    var file = file_node.files[0];
    var filereader = new FileReader();
    filereader.readAsDataURL(file);
    var self = this;
    filereader.onloadend = function (upload) {
      var data = filereader.result.toString();
      console.log(data);
      self.source = data;
      data = data.split(',')[1];
      self.propagateChange(data);
    };
  }

  onClear() {
    this.inputfile.nativeElement.value = "";
    this.source = this.placeholder;
    this.propagateChange(null);
  }

  //DRAG_DROP
  dropHandler(e) {
    console.log(e);
    e.stopPropagation();
    e.preventDefault();
    var file = e.dataTransfer.files[0];
    var filereader = new FileReader();
    filereader.readAsDataURL(file);
    var self = this;
    filereader.onloadend = function (upload) {
      var data = filereader.result.toString();
      console.log(data);
      self.source = data;
      data = data.split(',')[1];
      self.propagateChange(data);
    };
  }

  dragoverHandler(e) {
    console.log(e);
    e.stopPropagation();
    e.preventDefault();
    // Explicitly show this is a copy.
    e.dataTransfer.dropEffect = 'copy';
  }
}

