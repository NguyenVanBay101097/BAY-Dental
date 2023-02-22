import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantSaleOrderProcessUpdateListComponent } from './tenant-sale-order-process-update-list.component';

describe('TenantSaleOrderProcessUpdateListComponent', () => {
  let component: TenantSaleOrderProcessUpdateListComponent;
  let fixture: ComponentFixture<TenantSaleOrderProcessUpdateListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantSaleOrderProcessUpdateListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantSaleOrderProcessUpdateListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
