import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingRequestProductComponent } from './stock-picking-request-product.component';

describe('StockPickingRequestProductComponent', () => {
  let component: StockPickingRequestProductComponent;
  let fixture: ComponentFixture<StockPickingRequestProductComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingRequestProductComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingRequestProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
