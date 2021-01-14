import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingIncomingDetailComponent } from './stock-picking-incoming-detail.component';

describe('StockPickingIncomingDetailComponent', () => {
  let component: StockPickingIncomingDetailComponent;
  let fixture: ComponentFixture<StockPickingIncomingDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingIncomingDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingIncomingDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
