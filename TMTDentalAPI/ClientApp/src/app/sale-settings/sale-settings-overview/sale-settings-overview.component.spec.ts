import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleSettingsOverviewComponent } from './sale-settings-overview.component';

describe('SaleSettingsOverviewComponent', () => {
  let component: SaleSettingsOverviewComponent;
  let fixture: ComponentFixture<SaleSettingsOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleSettingsOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleSettingsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
