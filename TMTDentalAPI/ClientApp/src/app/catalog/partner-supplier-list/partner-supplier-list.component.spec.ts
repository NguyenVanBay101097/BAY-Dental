import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSupplierListComponent } from './partner-supplier-list.component';

describe('PartnerSupplierListComponent', () => {
  let component: PartnerSupplierListComponent;
  let fixture: ComponentFixture<PartnerSupplierListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSupplierListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSupplierListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
