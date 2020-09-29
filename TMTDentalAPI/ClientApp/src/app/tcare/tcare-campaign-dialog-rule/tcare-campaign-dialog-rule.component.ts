import { Component, OnInit, ViewChild, ComponentFactoryResolver, Type } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { AnchorHostDirective } from 'src/app/shared/anchor-host.directive';
import { AudienceFilterBirthdayComponent } from './audience-filter/audience-filter-dropdown/audience-filter-birthday/audience-filter-birthday.component';
import { Observable } from 'rxjs';
import { AudienceFilterLastTreatmentDayComponent } from './audience-filter/audience-filter-dropdown/audience-filter-last-treatment-day/audience-filter-last-treatment-day.component';
import { AudienceFilterServiceComponent } from './audience-filter/audience-filter-dropdown/audience-filter-service/audience-filter-service.component';
import { AudienceFilterPartnerCategoryComponent } from './audience-filter/audience-filter-dropdown/audience-filter-partner-category/audience-filter-partner-category.component';
import { AudienceFilterServiceCategoryComponent } from './audience-filter/audience-filter-dropdown/audience-filter-service-category/audience-filter-service-category.component';
import { AudienceFilterLastExaminationComponent } from './audience-filter/audience-filter-dropdown/audience-filter-last-examination/audience-filter-last-examination.component';
import { AudienceFilterAppointmentDayComponent } from './audience-filter/audience-filter-dropdown/audience-filter-appointment-day/audience-filter-appointment-day.component';

@Component({
  selector: 'app-tcare-campaign-dialog-rule',
  templateUrl: './tcare-campaign-dialog-rule.component.html',
  styleUrls: ['./tcare-campaign-dialog-rule.component.css']
})
export class TcareCampaignDialogRuleComponent implements OnInit {

  title: string;
  audience_filter: any;
  showAudienceFilter: boolean = false;
  formGroup: FormGroup;
  @ViewChild(AnchorHostDirective, { static: true }) anchorHost: AnchorHostDirective;

  xPos: number;
  yPos: number;

  conditionDropdownList: { type: string, name: string, component: Type<any> }[] = [
    {
      type: 'birthday',
      name: 'Sinh nhật',
      component: AudienceFilterBirthdayComponent
    }, 
    {
      type: 'lastSaleOrder',
      name: 'Ngày điều trị cuối',
      component: AudienceFilterLastTreatmentDayComponent
    },
    {
      type: 'lastExamination',
      name: 'Ngày khám cuối',
      component: AudienceFilterLastExaminationComponent
    },
    {
      type: 'lastAppointment',
      name: 'Lịch hẹn tiếp theo/gần đây',
      component: AudienceFilterAppointmentDayComponent
    }, 
    {
      type: 'categPartner',
      name: 'Nhãn khách hàng',
      component: AudienceFilterPartnerCategoryComponent
    }, 
    {
      type: 'usedService',
      name: 'Dịch vụ sử dụng',
      component: AudienceFilterServiceComponent
    }, 
    {
      type: 'usedCategService',
      name: 'Nhóm dịch vụ sử dụng',
      component: AudienceFilterServiceCategoryComponent
    }
  ];

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder,
    private componentFactoryResolver: ComponentFactoryResolver) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      logic: 'and',
      conditions: []
    });

    if (this.audience_filter) {
      this.formGroup.patchValue(this.audience_filter)
    }
  }

  getLogic() {
    var logic = this.formGroup.get('logic').value;
    if (logic == 'or') {
      return 'bất kì điều kiện nào';
    } else {
      return 'tất cả điều kiện';
    }
  }

  setLogic(logic) {
    this.formGroup.get('logic').setValue(logic);
  }

  removeCondition(index, event) {
    event.stopPropagation();
    this.conditions.splice(index, 1);
  }

  saveAudienceFilter(event) {
    this.audience_filter = event;
  }

  onSave() {
    var value = this.formGroup.value;
    this.activeModal.close(value);
  }

  outsideClick() {
    const viewContainerRef = this.anchorHost.viewContainerRef;
    viewContainerRef.clear();
  }

  getPosition(el) {
    var xPos = 0;
    var yPos = 0;
   
    while (el) {
      if (el.tagName == "BODY") {
        // deal with browser quirks with body/window/document and page scroll
        var xScroll = el.scrollLeft || document.documentElement.scrollLeft;
        var yScroll = el.scrollTop || document.documentElement.scrollTop;
   
        xPos += (el.offsetLeft - xScroll + el.clientLeft);
        yPos += (el.offsetTop - yScroll + el.clientTop);
      } else {
        // for all other non-BODY elements
        xPos += (el.offsetLeft - el.scrollLeft + el.clientLeft);
        yPos += (el.offsetTop - el.scrollTop + el.clientTop);
      }
   
      el = el.offsetParent;
    }

    return {
      x: xPos,
      y: yPos
    };
  }

  updatePosition(el) {
    var pos = this.getPosition(el);
    this.xPos = pos.x;
    this.yPos = pos.y + el.offsetHeight;
  }

  selectConditionDropdown(item: any, event) {
    event.stopPropagation();
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(item.component);
    const viewContainerRef = this.anchorHost.viewContainerRef;
    viewContainerRef.clear();

    const componentRef = viewContainerRef.createComponent(componentFactory);
    (<any>componentRef.instance).type = item.type;
    (<any>componentRef.instance).name = item.name;

    var saveClickObservable = (<any>componentRef.instance).saveClick as Observable<any>;
    if (saveClickObservable) {
      saveClickObservable.subscribe((result: any) => {
        console.log(result);
        viewContainerRef.clear();
        this.conditions.push(result);
      });
    }

    this.updatePosition(document.getElementById('conditionDropdown'));
  }

  get conditions() {
    return this.formGroup.get('conditions').value;
  }

  conditionClick(index, event) {
    event.stopPropagation();
    var condition = this.conditions[index];
    var items = this.conditionDropdownList.filter(x => x.type == condition.type);
    if (items.length) {
      var item = items[0];

      const componentFactory = this.componentFactoryResolver.resolveComponentFactory(item.component);
      const viewContainerRef = this.anchorHost.viewContainerRef;
      viewContainerRef.clear();
  
      const componentRef = viewContainerRef.createComponent(componentFactory);
      (<any>componentRef.instance).data = Object.assign({}, condition);
      (<any>componentRef.instance).type = condition.type;
      (<any>componentRef.instance).name = item.name;

      var saveClickObservable = (<any>componentRef.instance).saveClick as Observable<any>;
      if (saveClickObservable) {
        saveClickObservable.subscribe((result: any) => {
          viewContainerRef.clear();
          this.conditions[index] = result;
        });
      }

      this.updatePosition(event.currentTarget);
    }
  }
}
