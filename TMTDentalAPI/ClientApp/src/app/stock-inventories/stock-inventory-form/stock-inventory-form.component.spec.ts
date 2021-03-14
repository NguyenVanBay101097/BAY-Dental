import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockInventoryFormComponent } from './stock-inventory-form.component';

describe('StockInventoryFormComponent', () => {
  let component: StockInventoryFormComponent;
  let fixture: ComponentFixture<StockInventoryFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockInventoryFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockInventoryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
