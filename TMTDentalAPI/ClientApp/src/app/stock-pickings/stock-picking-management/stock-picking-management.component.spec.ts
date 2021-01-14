import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingManagementComponent } from './stock-picking-management.component';

describe('StockPickingManagementComponent', () => {
  let component: StockPickingManagementComponent;
  let fixture: ComponentFixture<StockPickingManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
