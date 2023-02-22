import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantExtendHistoryComponent } from './tenant-extend-history.component';

describe('TenantExtendHistoryComponent', () => {
  let component: TenantExtendHistoryComponent;
  let fixture: ComponentFixture<TenantExtendHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantExtendHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantExtendHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
