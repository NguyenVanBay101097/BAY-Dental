import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineManagementComponent } from './sale-order-line-management.component';

describe('SaleOrderLineManagementComponent', () => {
  let component: SaleOrderLineManagementComponent;
  let fixture: ComponentFixture<SaleOrderLineManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
