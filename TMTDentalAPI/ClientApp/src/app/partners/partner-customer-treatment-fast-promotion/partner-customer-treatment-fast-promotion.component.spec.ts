import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentFastPromotionComponent } from './partner-customer-treatment-fast-promotion.component';

describe('PartnerCustomerTreatmentFastPromotionComponent', () => {
  let component: PartnerCustomerTreatmentFastPromotionComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentFastPromotionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentFastPromotionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentFastPromotionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
