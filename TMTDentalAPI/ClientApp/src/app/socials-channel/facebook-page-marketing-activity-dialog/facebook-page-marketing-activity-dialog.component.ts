import { Component, OnInit, ViewChild, ComponentFactoryResolver, Output, EventEmitter, ComponentRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AnchorHostDirective } from 'src/app/shared/anchor-host.directive';
import { FacebookPageMarketingMessageAddButtonComponent } from '../facebook-page-marketing-message-add-button/facebook-page-marketing-message-add-button.component';

@Component({
  selector: 'app-facebook-page-marketing-activity-dialog',
  templateUrl: './facebook-page-marketing-activity-dialog.component.html',
  styleUrls: ['./facebook-page-marketing-activity-dialog.component.css']
})
export class FacebookPageMarketingActivityDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  activity: any;
  @ViewChild(AnchorHostDirective, { static: true }) anchorHost: AnchorHostDirective;

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal,
    private componentFactoryResolver: ComponentFactoryResolver) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      text: '',
      template: 'text',
      intervalNumber: 1,
      intervalType: 'days',
      buttons: this.fb.array([])
    });

    if (this.activity) {
      this.formGroup.patchValue(this.activity);
      if (this.activity.buttons) {
        this.activity.buttons.forEach(item => {
          this.buttonsFormArray.push(this.fb.group(item));
        });
      }
    }

    this.formGroup.get('text').valueChanges.subscribe((val: string) => {
      if (val.length > 640) {
        var newVal = val.substr(0, 640);
        this.formGroup.get('text').setValue(newVal);
      }
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    console.log(value);
    this.activeModal.close(value);
  }

  getLimitText() {
    var limit = 640;
    var text = this.formGroup.get('text').value;
    return limit - text.length;
  }

  get templateValue() {
    return this.formGroup.get('template').value;
  }

  get buttonsFormArray() {
    return this.formGroup.get('buttons') as FormArray;
  }

  addMessageButton(event: MouseEvent) {
    event.stopPropagation();

    var componentFactory = this.componentFactoryResolver.resolveComponentFactory(FacebookPageMarketingMessageAddButtonComponent);
    this.anchorHost.viewContainerRef.clear();
    const addButtonComponent: ComponentRef<FacebookPageMarketingMessageAddButtonComponent> = this.anchorHost.viewContainerRef.createComponent(componentFactory);
    addButtonComponent.instance.focusTextInput();

    addButtonComponent.instance.saveClick.subscribe(e => {
      this.buttonsFormArray.push(this.fb.group(e));
      this.anchorHost.viewContainerRef.clear();
    });

    addButtonComponent.instance.clickOutside.subscribe(() => {
      this.anchorHost.viewContainerRef.clear();
    });
  }

  editMessageButton(event: MouseEvent, control: FormControl) {
    event.stopPropagation();

    var componentFactory = this.componentFactoryResolver.resolveComponentFactory(FacebookPageMarketingMessageAddButtonComponent);
    this.anchorHost.viewContainerRef.clear();
    const addButtonComponent: ComponentRef<FacebookPageMarketingMessageAddButtonComponent> = this.anchorHost.viewContainerRef.createComponent(componentFactory);
    addButtonComponent.instance.data = control.value;
    addButtonComponent.instance.focusTextInput();

    addButtonComponent.instance.saveClick.subscribe(e => {
      control.patchValue(e);
      this.anchorHost.viewContainerRef.clear();
    });

    addButtonComponent.instance.clickOutside.subscribe(() => {
      this.anchorHost.viewContainerRef.clear();
    });
  }

  removeMessageButton(event, i) {
    event.stopPropagation();
    this.buttonsFormArray.removeAt(i);
  }
}
