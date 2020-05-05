import { Component, OnInit, ViewChild, ComponentFactoryResolver, Output, EventEmitter, ComponentRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AnchorHostDirective } from 'src/app/shared/anchor-host.directive';
import { FacebookPageMarketingMessageAddButtonComponent } from '../facebook-page-marketing-message-add-button/facebook-page-marketing-message-add-button.component';
import { MarketingCampaignActivitiesService } from '../marketing-campaign-activities.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-facebook-page-marketing-activity-dialog',
  templateUrl: './facebook-page-marketing-activity-dialog.component.html',
  styleUrls: ['./facebook-page-marketing-activity-dialog.component.css']
})
export class FacebookPageMarketingActivityDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  campaignId: string;
  activityId: string;
  parentId: string;
  triggerType: string;
  //
  activity: any;
  activities: any;
  audience_filter: any;
  showAudienceFilter: boolean = false;
  selectedTags: any[] = [];

  @ViewChild(AnchorHostDirective, { static: true }) anchorHost: AnchorHostDirective;

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal,
    private componentFactoryResolver: ComponentFactoryResolver, 
    private marketingCampaignActivitiesService: MarketingCampaignActivitiesService, 
    private notificationService: NotificationService ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      activityType: 'message',
      intervalType: 'days',
      intervalNumber: 1,
      template: 'text',
      text: '',
      triggerType: 'begin',
      parentId: null,
      campaignId: this.campaignId,
      actionType: '',
      tagIds: [],
      buttons: this.fb.array([])
    });

    if (this.activityId) {
      this.marketingCampaignActivitiesService.getWithID(this.activityId).subscribe(res => {
        this.activity = res;
        this.formGroup.patchValue(this.activity);
        this.selectedTags = this.activity.tags;
        // console.log(this.formGroup);  //
        // console.log(res); //
      }, err => {
        console.log(err);
      });
    }
    if (this.parentId) {
      this.formGroup.patchValue({
        triggerType: this.triggerType,
        parentId: this.parentId
      });
    }

    var val = {
      CampaignId: this.campaignId
    }
    this.marketingCampaignActivitiesService.get(val).subscribe((res: any) => {
      this.activities = res.items;
      // console.log(this.activities); // 
      if (this.activityId) {
        this.activities.splice(this.activities.findIndex(x => x.id === this.activityId), 1);
      }
    }, err => {
      console.log(err);
    });

    // if (this.activity) {
    //   this.formGroup.patchValue(this.activity);
    //   console.log(this.formGroup); //
    //   if (this.activity.buttons) {
    //     this.activity.buttons.forEach(item => {
    //       this.buttonsFormArray.push(this.fb.group(item));
    //     });
    //   }
    // }

    this.formGroup.get('text').valueChanges.subscribe((val: string) => {
      if (val && val.length > 640) {
        var newVal = val.substr(0, 640);
        this.formGroup.get('text').setValue(newVal);
      }
    });

    this.showAudienceFilter = true;
  }

  get activityTypeValue() {
    return this.formGroup.get('activityType').value;
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    if (this.formGroup.value.triggerType === "begin") {
      this.formGroup.patchValue({
        parentId: null
      });
    }
    //
    var tagIds = [];
    for (let i = 0; i < this.selectedTags.length; i++) {
      tagIds.push(this.selectedTags[i].id);
    }
    this.formGroup.patchValue({
      tagIds: tagIds
    });
    var value = this.formGroup.value;
    
    if (this.activityId) {
      console.log(this.activityId);
      this.marketingCampaignActivitiesService.put(this.activityId, value).subscribe(res => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.activeModal.close("loading");
      }, err => {
        console.log(err);
        this.activeModal.dismiss();
      });
    } else {
      this.marketingCampaignActivitiesService.post(value).subscribe(res => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.activeModal.close("loading");
      }, err => {
        console.log(err);
        this.activeModal.dismiss();
      });
    }
  }

  getLimitText() {
    var limit = 640;
    var text = this.formGroup.get('text').value;
    if (text) {
      return limit - text.length;
    } else {
      return limit;
    }
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

  save_selectedTags(event) {
    this.selectedTags = event;
    // console.log(event);
  }

  saveAudienceFilter(event) {
    this.audience_filter = event;
  }

  showTags() {
    if (this.formGroup.value.activityType === "action") {
      if (this.formGroup.value.actionType === "Add Tag" || this.formGroup.value.actionType === "Delete Tag") {
        return true;
      }
    }
    return false;
  }

  showActivities() {
    if (this.formGroup.value.triggerType === "act" || this.formGroup.value.triggerType === "message_open") {
      return true;
    }
    return false;
  }
}
