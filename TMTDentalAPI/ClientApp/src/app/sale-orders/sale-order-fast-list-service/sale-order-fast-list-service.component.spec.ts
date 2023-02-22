import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderFastListServiceComponent } from './sale-order-fast-list-service.component';

describe('SaleOrderFastListServiceComponent', () => {
  let component: SaleOrderFastListServiceComponent;
  let fixture: ComponentFixture<SaleOrderFastListServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderFastListServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderFastListServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
