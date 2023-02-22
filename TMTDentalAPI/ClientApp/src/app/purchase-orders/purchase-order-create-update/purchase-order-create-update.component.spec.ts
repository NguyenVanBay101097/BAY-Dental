import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOrderCreateUpdateComponent } from './purchase-order-create-update.component';

describe('PurchaseOrderCreateUpdateComponent', () => {
  let component: PurchaseOrderCreateUpdateComponent;
  let fixture: ComponentFixture<PurchaseOrderCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseOrderCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseOrderCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
