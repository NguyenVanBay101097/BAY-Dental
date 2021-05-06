import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentLineFastPromotionComponent } from './partner-customer-treatment-line-fast-promotion.component';

describe('PartnerCustomerTreatmentLineFastPromotionComponent', () => {
  let component: PartnerCustomerTreatmentLineFastPromotionComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentLineFastPromotionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentLineFastPromotionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentLineFastPromotionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
