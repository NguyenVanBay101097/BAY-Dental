import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerCategoriesComponent } from './partner-customer-categories.component';

describe('PartnerCustomerCategoriesComponent', () => {
  let component: PartnerCustomerCategoriesComponent;
  let fixture: ComponentFixture<PartnerCustomerCategoriesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerCategoriesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerCategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
