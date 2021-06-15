import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderManagementComponent } from './sale-order-management.component';

describe('SaleOrderManagementComponent', () => {
  let component: SaleOrderManagementComponent;
  let fixture: ComponentFixture<SaleOrderManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
