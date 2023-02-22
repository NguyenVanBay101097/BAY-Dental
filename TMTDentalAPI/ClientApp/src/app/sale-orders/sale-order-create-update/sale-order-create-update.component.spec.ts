import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderCreateUpdateComponent } from './sale-order-create-update.component';

describe('SaleOrderCreateUpdateComponent', () => {
  let component: SaleOrderCreateUpdateComponent;
  let fixture: ComponentFixture<SaleOrderCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
