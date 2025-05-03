using BuildingUtils;
using UnityEngine;

static class Extensions
{


    public static CellType as_cell_type(this BuildingModel model)
    {
        if (BuildingModel.BUILDING_MIN < model && model < BuildingModel.BUILDING_MAX)
        {
            return CellType.Building;
        }
        else if (BuildingModel.ROAD_MIN < model && model < BuildingModel.ROAD_MAX)
        {
            return CellType.Road;
        }
        if (model == BuildingModel.NONE)
        {
            return CellType.None;
        }
        else
        {
            throw new UnityException($"UNREACHABLE: Does BuildingModel.as_cell_type({model}) make sense?");
        }
    }

    public static BuildingModel as_model_from_neighbor_count(this int neighbors)
    {
        switch (neighbors)
        {
            case 1:
                return BuildingModel.Deadend;
            case 2:
                return BuildingModel.Straight;
            case 3:
                return BuildingModel.ThreeWay;
            case 4:
                return BuildingModel.FourWay;
            default:
                return BuildingModel.NONE;
        }

    }

    public static int appropriate_neighbor_count(this BuildingModel model)
    {
        switch (model)
        {
            case BuildingModel.Deadend:
                return 1;
            case BuildingModel.Curve:
            case BuildingModel.Straight:
                return 2;
            case BuildingModel.ThreeWay:
                return 3;
            case BuildingModel.FourWay:
                return 4;
            default:
                return 1;
        }
    }

    public static bool is_building(this CellType ct)
    {
        return ct == CellType.Building;
    }

    public static bool is_none(this CellType ct)
    {
        return ct == CellType.None;
    }
    public static bool is_road(this CellType ct)
    {
        return ct == CellType.Road;
    }

    public static BuildingModel as_building_model(this int val)
    {
        if ((val + ((int)BuildingModel.BUILDING_MIN + 1)) > ((int)BuildingModel.BUILDING_MAX))
            val = 0;
        return (BuildingModel)(val + ((int)BuildingModel.BUILDING_MIN + 1));
    }

    public static BuildingModel as_road_model(this int val)
    {
        if ((val + ((int)BuildingModel.ROAD_MIN + 1)) > ((int)BuildingModel.ROAD_MAX))
            val = 0;
        return (BuildingModel)(val + ((int)BuildingModel.ROAD_MIN + 1));
    }

    public static BuildingInfo get_building_info(this BuildingModel model)
    {
        BuildingInfo info = new BuildingInfo();
        switch (model)
        {
            case BuildingModel.Bank:
                info.air_pollution = 6;
                info.noise_pollution = 4;
                info.power_usage = 8;
                info.max_capacity = 0;
                break;
            case BuildingModel.Flat_1:
                info.air_pollution = 7;
                info.noise_pollution = 13;
                info.power_usage = 15;
                info.max_capacity = 15;
                break;
            case BuildingModel.Flat_2:
                info.air_pollution = 7;
                info.noise_pollution = 13;
                info.power_usage = 15;
                info.max_capacity = 15;
                break;
            case BuildingModel.House_1:
                info.air_pollution = 3;
                info.noise_pollution = 3;
                info.power_usage = 5;
                info.max_capacity = 5;
                break;
            case BuildingModel.House_2:
                info.air_pollution = 4;
                info.noise_pollution = 3;
                info.power_usage = 6;
                info.max_capacity = 7;
                break;
            case BuildingModel.House_3:
                info.air_pollution = 5;
                info.noise_pollution = 3;
                info.power_usage = 7;
                info.max_capacity = 8;
                break;
            case BuildingModel.Shop:
                info.air_pollution = 6;
                info.noise_pollution = 4;
                info.power_usage = 5;
                info.max_capacity = 0;
                break;
        }
        return info;
    }

    public static string into_serial(this Cell cell)
    {
        CellSerial cs = new CellSerial();
        cs.location = cell.location;
        cs.zone_type = cell.zone_type;
        cs.color = cell.color;
        cs.cell_type = cell.cell_type;
        cs.building_model = cell.contents?.model ?? BuildingModel.NONE;
        cs.rotation = cell.contents?.get_rotation() ?? Quaternion.identity;
        return JsonUtility.ToJson(cs);
    }

    /*public static string into_serial(this Citizen citizen)*/
    /*{*/
    /*    CitizenSerial citizen_ser = new CitizenSerial();*/
    /*    citizen_ser.curr_dest = citizen.destination;*/
    /*    citizen_ser.prefab_idx = citizen.model;*/
    /*    return JsonUtility.ToJson(citizen_ser);*/
    /*}*/
    /**/
    /*public static void from_serial(this Citizen citizen, string citizen_serial)*/
    /*{*/
    /*}*/

    public static void from_serial(this Cell c, string cell_serial)
    {
        CellSerial cs = JsonUtility.FromJson<CellSerial>(cell_serial);
        c.location = cs.location;
        c.cell_type = cs.cell_type;
        c.SetZoneTypeAndUpdate(cs.zone_type);
        if (cs.building_model is BuildingModel model && model != BuildingModel.NONE)
        {
            c.FromModelAndUpdate(model);
            c.contents.apply_rotation(cs.rotation);
        }
        else
        {
            c.SetWalkableAndUpdate(false);
        }
    }

}
