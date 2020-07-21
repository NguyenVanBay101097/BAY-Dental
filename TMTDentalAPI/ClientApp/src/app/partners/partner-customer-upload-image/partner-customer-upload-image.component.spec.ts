import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerUploadImageComponent } from './partner-customer-upload-image.component';

describe('PartnerCustomerUploadImageComponent', () => {
  let component: PartnerCustomerUploadImageComponent;
  let fixture: ComponentFixture<PartnerCustomerUploadImageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerUploadImageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerUploadImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
